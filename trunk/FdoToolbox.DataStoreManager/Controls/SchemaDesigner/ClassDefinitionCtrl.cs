﻿#region LGPL Header
// Copyright (C) 2010, Jackie Ng
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OSGeo.FDO.Schema;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.DataStoreManager.Controls.SchemaDesigner
{
    internal partial class ClassDefinitionCtrl : UserControl
    {
        private ClassDefinitionCtrl()
        {
            InitializeComponent();
        }

        private SchemaDesignContext _context;
        private ClassDefinitionDecorator _cls;

        private ClassDefinitionCtrl(ClassDefinitionDecorator cls, SchemaDesignContext context)
            : this()
        {
            _cls = cls;
            _context = context;
            txtName.DataBindings.Add("Text", cls, "Name");
            txtDescription.DataBindings.Add("Text", cls, "Description");

            txtType.Text = cls.ClassType.ToString();

            BindIdentityProperties(cls);

            //cmbBaseClass.DisplayMember = "Name";
            //cmbBaseClass.DataSource = _context.GetClassesExceptFor(cls.ParentName, cls.Name);
            //cmbBaseClass.DataBindings.Add("SelectedItem", cls, "BaseClass");

            chkAbstract.DataBindings.Add("Checked", cls, "IsAbstract");
            chkComputed.DataBindings.Add("Checked", cls, "IsComputed");
        }

        private void BindIdentityProperties(ClassDefinitionDecorator cls)
        {
            //Fill the list
            foreach (PropertyDefinition p in cls.Properties)
            {
                if (p.PropertyType == PropertyType.PropertyType_DataProperty)
                {
                    chkIdentityProperties.Items.Add(p.Name, false);
                }
            }

            //Now check the ones which are identity
            foreach (DataPropertyDefinition p in cls.IdentityProperties)
            {
                var idx = chkIdentityProperties.Items.IndexOf(p.Name);
                if (idx >= 0)
                    chkIdentityProperties.SetItemChecked(idx, true);
            }

            //Now wire up change listener
            chkIdentityProperties.ItemCheck += (s, e) =>
            {
                var idx = e.Index;
                string name = chkIdentityProperties.Items[idx].ToString();
                if (e.NewValue == CheckState.Checked)
                {
                    cls.MarkAsIdentity(name);
                }
                else if (e.NewValue == CheckState.Unchecked)
                {
                    cls.RemoveIdentityProperty(name);
                }
            };
        }

        public ClassDefinitionCtrl(ClassDecorator cls, SchemaDesignContext context)
            : this((ClassDefinitionDecorator)cls, context)
        {
            cmbGeometricProperty.Enabled = false;
        }

        public ClassDefinitionCtrl(FeatureClassDecorator cls, SchemaDesignContext context)
            : this((ClassDefinitionDecorator)cls, context)
        {
            cmbGeometricProperty.DataSource = cls.AvailableGeometricProperties;
            cmbGeometricProperty.SelectionChangeCommitted += (s, e) =>
            {
                if (cmbGeometricProperty.SelectedItem != null)
                {
                    cls.AssignGeometricProperty(cmbGeometricProperty.SelectedItem.ToString());
                }
            };
        }
    }
}
