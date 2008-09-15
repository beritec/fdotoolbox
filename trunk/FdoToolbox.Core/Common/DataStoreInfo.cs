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
using OSGeo.FDO.Commands.DataStore;

namespace FdoToolbox.Core
{
    /// <summary>
    /// Data transfer object for FDO Data store information
    /// </summary>
    public class DataStoreInfo
    {
        private string _Name;

        /// <summary>
        /// The name of the data store
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description;

        /// <summary>
        /// The data store description
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public DataStoreInfo(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public DataStoreInfo(IDataStoreReader reader)
        {
            this.Name = reader.GetName();
            this.Description = reader.GetDescription();
        }
    }
}
