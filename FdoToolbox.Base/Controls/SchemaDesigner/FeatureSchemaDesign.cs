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
using OSGeo.FDO.Schema;
using System.ComponentModel;

namespace FdoToolbox.Base.Controls.SchemaDesigner
{
    internal class FeatureSchemaDesign : INotifyPropertyChanged
    {
        private FeatureSchema _schema;

        public FeatureSchemaDesign(FeatureSchema schema)
        {
            _schema = schema;
        }

        internal FeatureSchema WrappedSchema
        {
            get { return _schema; }
        }

        [Description("The name of the schema")]
        public string Name
        {
            get { return _schema.Name; }
            set 
            { 
                _schema.Name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        [Description("Schema description")]
        public string Description
        {
            get { return _schema.Description; }
            set { _schema.Description = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
