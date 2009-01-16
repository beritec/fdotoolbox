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
using FdoToolbox.Core.Feature;
using OSGeo.FDO.Commands.Feature;
using System.Collections.Specialized;
using OSGeo.FDO.Commands;
using OSGeo.FDO.Expression;
using OSGeo.FDO.Schema;
using Iesi.Collections.Generic;

namespace FdoToolbox.Core.ETL.Operations
{
    /// <summary>
    /// Output pipeline operation
    /// </summary>
    public class FdoOutputOperation : FdoOperationBase
    {
        /// <summary>
        /// The output connection
        /// </summary>
        protected FdoConnection _conn;
        /// <summary>
        /// The service bound to the output connection
        /// </summary>
        protected FdoFeatureService _service;
        /// <summary>
        /// The property value mappings
        /// </summary>
        protected NameValueCollection _mappings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        public FdoOutputOperation(FdoConnection conn, string className)
        {
            _conn = conn;
            _service = conn.CreateFeatureService();
            this.ClassName = className;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="className"></param>
        /// <param name="propertyMappings"></param>
        public FdoOutputOperation(FdoConnection conn, string className, NameValueCollection propertyMappings)
            : this(conn, className)
        {
            _mappings = propertyMappings;
        }

        private string _ClassName;

        /// <summary>
        /// The name of the feature class to write features to
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; }
        }

        protected HashedSet<string> _unWritableProperties = new HashedSet<string>();

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter"></param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            //Omit read-only properties
            using (FdoFeatureService service = _conn.CreateFeatureService())
            {
                ClassDefinition c = service.GetClassByName(this.ClassName);
                foreach (PropertyDefinition p in c.Properties)
                {
                    string name = p.Name;
                    if (p.PropertyType != PropertyType.PropertyType_DataProperty && p.PropertyType != PropertyType.PropertyType_GeometricProperty)
                    {
                        _unWritableProperties.Add(name);
                    }
                    else
                    {
                        if (p.PropertyType == PropertyType.PropertyType_GeometricProperty)
                        {
                            GeometricPropertyDefinition g = p as GeometricPropertyDefinition;
                            if (g.ReadOnly)
                                _unWritableProperties.Add(name);
                        }
                        else
                        {
                            DataPropertyDefinition d = p as DataPropertyDefinition;
                            if (d.ReadOnly) //|| d.IsAutoGenerated)
                                _unWritableProperties.Add(name);
                        }
                    }
                }
                c.Dispose();
            }

            base.PrepareForExecution(pipelineExecuter);
        }

        /// <summary>
        /// Executes the operation
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<FdoRow> Execute(IEnumerable<FdoRow> rows)
        {
            foreach (FdoRow row in rows)
            {
                using (PropertyValueCollection propVals = row.ToPropertyValueCollection(_mappings, _unWritableProperties))
                {
                    _service.InsertFeature(this.ClassName, propVals, false);
                    RaiseFeatureProcessed(row);
                }
            }
            yield break;
        }
    }
}
