#region LGPL Header
// Copyright (C) 2008, Jackie Ng
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
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.FDO.Connections;
using FdoToolbox.Core.ETL;

namespace FdoToolbox.Core.Utility
{
    /// <summary>
    /// Utility class to create override objects
    /// </summary>
    public sealed class OverrideFactory
    {
        private static Dictionary<string, Type> _CopySpatialContextOverrides;

        private static Dictionary<string, Type> _ClassNameOverrides;

        static OverrideFactory()
        {
            _CopySpatialContextOverrides = new Dictionary<string, Type>();
            _ClassNameOverrides = new Dictionary<string, Type>();

            RegisterClassNameOverride("OSGeo.KingOracle", typeof(OracleClassNameOverride));
            RegisterCopySpatialContextOverride("OSGeo.MySQL", typeof(MySqlCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SHP", typeof(ShpCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SQLServerSpatial", typeof(MsSqlCopySpatialContextOverride));

            RegisterClassNameOverride("OSGeo.KingOracle.3.3", typeof(OracleClassNameOverride));
            RegisterCopySpatialContextOverride("OSGeo.MySQL.3.3", typeof(MySqlCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SHP.3.3", typeof(ShpCopySpatialContextOverride));
            RegisterCopySpatialContextOverride("OSGeo.SQLServerSpatial.3.3", typeof(MsSqlCopySpatialContextOverride));
        }

        public static void RegisterCopySpatialContextOverride(string providerName, Type overrideType)
        {
            if(!Array.Exists<Type>(overrideType.GetInterfaces(), delegate(Type t) { return t == typeof(ICopySpatialContextOverride); }))
                throw new ArgumentException("The given type does not implement ICopySpatialContextOverride");

            _CopySpatialContextOverrides[providerName] = overrideType;
        }

        public static void RegisterClassNameOverride(string providerName, Type overrideType)
        {
            if (!Array.Exists<Type>(overrideType.GetInterfaces(), delegate(Type t) { return t == typeof(IClassNameOverride); }))
                throw new ArgumentException("The given type does not implement IClassNameOverride");

            _ClassNameOverrides[providerName] = overrideType;
        }

        public static ICopySpatialContextOverride GetCopySpatialContextOverride(IConnection targetConn)
        {
            string providerName = targetConn.ConnectionInfo.ProviderName;
            if (_CopySpatialContextOverrides.ContainsKey(providerName))
            {
                return (ICopySpatialContextOverride)Activator.CreateInstance(_CopySpatialContextOverrides[providerName]);
            }
            return null;
        }

        public static IClassNameOverride GetClassNameOverride(IConnection conn)
        {
            string providerName = conn.ConnectionInfo.ProviderName;
            if (_ClassNameOverrides.ContainsKey(providerName))
            {
                return (IClassNameOverride)Activator.CreateInstance(_ClassNameOverrides[providerName]);
            }
            return null;
        }
    }
}
