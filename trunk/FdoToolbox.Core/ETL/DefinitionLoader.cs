#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using FdoToolbox.Core.ETL.Specialized;
using FdoToolbox.Core.Configuration;
using FdoToolbox.Core.Feature;
using System.Xml.Serialization;
using System.IO;
using OSGeo.FDO.Filter;
using System.Xml;
using System.Collections.Specialized;
using OSGeo.FDO.Schema;
using Iesi.Collections.Generic;

namespace FdoToolbox.Core.ETL
{
    internal class FeatureSchemaCache : IDisposable
    {
        private Dictionary<string, FeatureSchemaCollection> _cache;

        public FeatureSchemaCache()
        {
            _cache = new Dictionary<string, FeatureSchemaCollection>();
        }

        public void Add(string name, FeatureSchemaCollection schemas)
        {
            _cache[name] = schemas;
        }

        public ClassDefinition GetClassByName(string name, string schemaName, string className)
        {
            if (!_cache.ContainsKey(name))
                return null;

            FeatureSchemaCollection item = _cache[name];

            int sidx = item.IndexOf(schemaName);
            if (sidx >= 0)
            {
                var classes = item[sidx].Classes;
                int cidx = classes.IndexOf(className);

                if (cidx >= 0)
                    return classes[cidx];
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var key in _cache.Keys)
            {
                _cache[key].Dispose();
            }
            _cache.Clear();
        }

        internal bool HasConnection(string connName)
        {
            return _cache.ContainsKey(connName);
        }
    }

    internal class MultiSchemaQuery
    {
        private List<SchemaQuery> _query;

        public MultiSchemaQuery(string connName)
        {
            this.ConnectionName = connName;
            _query = new List<SchemaQuery>();
        }

        public string ConnectionName { get; set; }

        public SchemaQuery TryGet(string schemaName)
        {
            foreach (var q in _query)
            {
                if (q.SchemaName == schemaName)
                    return q;
            }
            return null;
        }

        public void Add(SchemaQuery query)
        {
            _query.Add(query);
        }

        public SchemaQuery[] SchemaQueries { get { return _query.ToArray(); } }
    }

    internal class SchemaQuery
    {
        public SchemaQuery(string schemaName)
        {
            this.SchemaName = schemaName;
            _classes = new HashedSet<string>();
        }

        private HashedSet<string> _classes;

        public string SchemaName { get; set; }

        public void AddClass(string name)
        {
            _classes.Add(name);
        }

        public IEnumerable<string> ClassNames
        {
            get { return _classes; }
        }
    }

    /// <summary>
    /// Task definition loader base class
    /// </summary>
    public abstract class BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The connection string.</param>
        /// <param name="name">The name that will be assigned to the connection.</param>
        /// <returns></returns>
        protected abstract FdoConnection CreateConnection(string provider, string connStr, ref string name);

        /// <summary>
        /// Prepares the specified bulk copy definition (freshly deserialized) before the loading process begins
        /// </summary>
        /// <param name="def">The bulk copy definition.</param>
        protected abstract void Prepare(FdoBulkCopyTaskDefinition def);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDefinitionLoader"/> class.
        /// </summary>
        protected BaseDefinitionLoader() { }

        /// <summary>
        /// Loads bulk copy options from xml
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoBulkCopyOptions BulkCopyFromXml(string file, ref string name, bool owner)
        {
            FdoBulkCopyTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoBulkCopyTaskDefinition));
            def = (FdoBulkCopyTaskDefinition)ser.Deserialize(new StreamReader(file));
            
            return BulkCopyFromXml(def, ref name, owner);
        }

        /// <summary>
        /// Loads bulk copy options from deserialized xml
        /// </summary>
        /// <param name="def">The deserialized definition.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoBulkCopyOptions BulkCopyFromXml(FdoBulkCopyTaskDefinition def, ref string name, bool owner)
        {
            Prepare(def);

            name = def.name;
            Dictionary<string, FdoConnection> connections = new Dictionary<string, FdoConnection>();
            Dictionary<string, string> changeConnNames = new Dictionary<string, string>();

            foreach (FdoConnectionEntryElement entry in def.Connections)
            {
                string connName = entry.name;
                FdoConnection conn = CreateConnection(entry.provider, entry.ConnectionString, ref connName);
                connections[connName] = conn;

                if (connName != entry.name)
                {
                    changeConnNames[entry.name] = connName;
                }
            }
            
            foreach (string oldName in changeConnNames.Keys)
            {
                def.UpdateConnectionReferences(oldName, changeConnNames[oldName]);
            }

            //Compile the list of classes to be queried
            Dictionary<string, MultiSchemaQuery> queries = new Dictionary<string, MultiSchemaQuery>();
            foreach (FdoCopyTaskElement task in def.CopyTasks)
            {
                MultiSchemaQuery src = null;
                MultiSchemaQuery dst = null;

                //Process source
                if (!queries.ContainsKey(task.Source.connection))
                    src = queries[task.Source.connection] = new MultiSchemaQuery(task.Source.connection);
                else
                    src = queries[task.Source.connection];

                var sq = src.TryGet(task.Source.schema);
                if (sq != null)
                {
                    sq.AddClass(task.Source.@class);
                }
                else
                {
                    sq = new SchemaQuery(task.Source.schema);
                    sq.AddClass(task.Source.@class);
                    src.Add(sq);
                }

                //Process target
                if (!queries.ContainsKey(task.Target.connection))
                    dst = queries[task.Target.connection] = new MultiSchemaQuery(task.Target.connection);
                else
                    dst = queries[task.Target.connection];

                var tq = dst.TryGet(task.Target.schema);
                if (tq != null)
                {
                    tq.AddClass(task.Target.@class);
                }
                else
                {
                    tq = new SchemaQuery(task.Target.schema);
                    tq.AddClass(task.Target.@class);
                    dst.Add(tq);
                }
            }

            var schemaCache = new FeatureSchemaCache();

            //Now populate the schema cache
            foreach (string connName in queries.Keys)
            {
                if (connections.ContainsKey(connName))
                {
                    var mqry = queries[connName];
                    var conn = connections[connName];

                    FeatureSchemaCollection schemas = new FeatureSchemaCollection(null);

                    using (var svc = conn.CreateFeatureService())
                    {
                        foreach (var sq in mqry.SchemaQueries)
                        {
                            schemas.Add(svc.PartialDescribeSchema(sq.SchemaName, new List<string>(sq.ClassNames)));
                        }
                    }

                    schemaCache.Add(connName, schemas);
                }
            }

            
            FdoBulkCopyOptions opts = new FdoBulkCopyOptions(connections, owner);
            using (schemaCache)
            {
                foreach (FdoCopyTaskElement task in def.CopyTasks)
                {
                    FdoClassCopyOptions copt = FdoClassCopyOptions.FromElement(task, schemaCache, connections[task.Source.connection]);
                    opts.AddClassCopyOption(copt);
                }
            }
            return opts;
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        public FdoJoinOptions JoinFromXml(string file, ref string name, bool owner)
        {
            FdoJoinTaskDefinition def = null;
            XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
            def = (FdoJoinTaskDefinition)ser.Deserialize(new StreamReader(file));

            return JoinFromXml(def, ref name, owner);
        }

        /// <summary>
        /// Loads join options from xml
        /// </summary>
        /// <param name="def">The deserialized definition</param>
        /// <param name="name">The name.</param>
        /// <param name="owner">if set to <c>true</c> [owner].</param>
        /// <returns></returns>
        private FdoJoinOptions JoinFromXml(FdoJoinTaskDefinition def, ref string name, bool owner)
        {
            FdoJoinOptions opts = new FdoJoinOptions(owner);
            name = def.name;
            if (def.JoinSettings.DesignatedGeometry != null)
            {
                opts.GeometryProperty = def.JoinSettings.DesignatedGeometry.Property;
                opts.Side = def.JoinSettings.DesignatedGeometry.Side;
            }
            foreach (JoinKey key in def.JoinSettings.JoinKeys)
            {
                opts.JoinPairs.Add(key.left, key.right);
            }
            string dummy = string.Empty;
            opts.JoinType = (FdoJoinType)Enum.Parse(typeof(FdoJoinType), def.JoinSettings.JoinType.ToString());
            opts.SetLeft(
                CreateConnection(def.Left.Provider, def.Left.ConnectionString, ref dummy),
                def.Left.FeatureSchema,
                def.Left.Class);
            foreach (string p in def.Left.PropertyList)
            {
                opts.AddLeftProperty(p);
            }
            opts.SetRight(
                CreateConnection(def.Right.Provider, def.Right.ConnectionString, ref dummy),
                def.Right.FeatureSchema,
                def.Right.Class);
            foreach (string p in def.Right.PropertyList)
            {
                opts.AddRightProperty(p);
            }

            opts.SetTarget(
                CreateConnection(def.Target.Provider, def.Target.ConnectionString, ref dummy),
                def.Target.FeatureSchema,
                def.Target.Class);

            opts.LeftPrefix = def.Left.Prefix;
            opts.RightPrefix = def.Right.Prefix;
            opts.ForceOneToOne = def.JoinSettings.ForceOneToOne;
            if (def.JoinSettings.SpatialPredicateSpecified)
                opts.SpatialJoinPredicate = (SpatialOperations)Enum.Parse(typeof(SpatialOperations), def.JoinSettings.SpatialPredicate.ToString());

            return opts;
        }

        /// <summary>
        /// Saves the join options to xml
        /// </summary>
        /// <param name="opts">The opts.</param>
        /// <param name="name">The name.</param>
        /// <param name="file">The file.</param>
        public void ToXml(FdoJoinOptions opts, string name, string file)
        {
            FdoJoinTaskDefinition jdef = new FdoJoinTaskDefinition();
            jdef.name = name;
            jdef.JoinSettings = new FdoJoinSettings();
            if (!string.IsNullOrEmpty(opts.GeometryProperty))
            {
                jdef.JoinSettings.DesignatedGeometry = new FdoDesignatedGeometry();
                jdef.JoinSettings.DesignatedGeometry.Property = opts.GeometryProperty;
                jdef.JoinSettings.DesignatedGeometry.Side = opts.Side;
            }
            List<JoinKey> keys = new List<JoinKey>();
            foreach (string left in opts.JoinPairs.Keys)
            {
                JoinKey key = new JoinKey();
                key.left = left;
                key.right = opts.JoinPairs[left];
                keys.Add(key);
            }
            jdef.JoinSettings.JoinKeys = keys.ToArray();

            jdef.Left = new FdoJoinSource();
            jdef.Left.Class = opts.Left.ClassName;
            jdef.Left.ConnectionString = opts.Left.Connection.ConnectionString;
            jdef.Left.FeatureSchema = opts.Left.SchemaName;
            jdef.Left.Prefix = opts.LeftPrefix;
            jdef.Left.PropertyList = new List<string>(opts.LeftProperties).ToArray();
            jdef.Left.Provider = opts.Left.Connection.Provider;

            jdef.Right = new FdoJoinSource();
            jdef.Right.Class = opts.Right.ClassName;
            jdef.Right.ConnectionString = opts.Right.Connection.ConnectionString;
            jdef.Right.FeatureSchema = opts.Right.SchemaName;
            jdef.Right.Prefix = opts.RightPrefix;
            jdef.Right.PropertyList = new List<string>(opts.RightProperties).ToArray();
            jdef.Right.Provider = opts.Right.Connection.Provider;

            jdef.Target = new FdoJoinTarget();
            jdef.Target.Class = opts.Target.ClassName;
            jdef.Target.ConnectionString = opts.Target.Connection.ConnectionString;
            jdef.Target.FeatureSchema = opts.Target.SchemaName;
            jdef.Target.Provider = opts.Target.Connection.Provider;

            using (XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;

                XmlSerializer ser = new XmlSerializer(typeof(FdoJoinTaskDefinition));
                ser.Serialize(writer, jdef);
            }
        }
    }

    /// <summary>
    /// Handler for generating connection names
    /// </summary>
    public delegate string ConnectionNameGenerationCallback(int seed);

    /// <summary>
    /// Helper class for Task Definition serialization
    /// </summary>
    public sealed class TaskDefinitionHelper
    {
        /// <summary>
        /// File extension for bulk copy definitions
        /// </summary>
        public const string BULKCOPYDEFINITION = ".BulkCopyDefinition";
        /// <summary>
        /// File extension for join definitions
        /// </summary>
        public const string JOINDEFINITION = ".JoinDefinition";

        /// <summary>
        /// Determines whether [the specified file] is a definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDefinitionFile(string file)
        {
            if (!File.Exists(file))
                return false;

            string ext = Path.GetExtension(file).ToLower();

            return (ext == BULKCOPYDEFINITION.ToLower()) || (ext == JOINDEFINITION.ToLower());
        }

        /// <summary>
        /// Determines whether [the specified file] is a bulk copy definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified file] is a bulk copy definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBulkCopy(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == BULKCOPYDEFINITION.ToLower());
        }

        /// <summary>
        /// Determines whether the specified file is a join definition
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// 	<c>true</c> if the specified file is a join definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJoin(string file)
        {
            return IsDefinitionFile(file) && (Path.GetExtension(file).ToLower() == JOINDEFINITION.ToLower());
        }
    }

    /// <summary>
    /// Standalone task definition loader. Use this loader when using only the Core API. 
    /// Do not use this loader in the context of the FDO Toolbox application.
    /// </summary>
    public class DefinitionLoader : BaseDefinitionLoader
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="name">The name that will be assigned to the connection.</param>
        /// <returns></returns>
        protected override FdoConnection CreateConnection(string provider, string connStr, ref string name)
        {
            return new FdoConnection(provider, connStr);
        }

        /// <summary>
        /// Prepares the specified bulk copy definition (freshly deserialized) before the loading process begins
        /// </summary>
        /// <param name="def">The bulk copy definition.</param>
        protected override void Prepare(FdoBulkCopyTaskDefinition def)
        {
            
        }
    }
}
