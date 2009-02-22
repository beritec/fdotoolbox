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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Services;

namespace FdoToolbox.Base.Controls
{
    public partial class ViewContent : UserControl
    {
        private IFdoConnectionManager connMgr;

        public ViewContent()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(OnDisposed);
            connMgr = ServiceManager.Instance.GetService<IFdoConnectionManager>();
            connMgr.BeforeConnectionRemove += new ConnectionBeforeRemoveHandler(OnBeforeConnectionRemove);
            connMgr.ConnectionAdded += new ConnectionEventHandler(OnConnectionAdded);
            connMgr.ConnectionRefreshed += new ConnectionEventHandler(OnConnectionRefreshed);
            connMgr.ConnectionRemoved += new ConnectionEventHandler(OnConnectionRemoved);
            connMgr.ConnectionRenamed += new ConnectionRenamedEventHandler(OnConnectionRenamed);
        }

        protected virtual void OnConnectionRenamed(object sender, ConnectionRenameEventArgs e) { }

        protected virtual void OnConnectionRemoved(object sender, FdoToolbox.Core.EventArgs<string> e) { }

        protected virtual void OnConnectionRefreshed(object sender, FdoToolbox.Core.EventArgs<string> e) { }

        protected virtual void OnConnectionAdded(object sender, FdoToolbox.Core.EventArgs<string> e) { }

        protected virtual void OnBeforeConnectionRemove(object sender, ConnectionBeforeRemoveEventArgs e) { }

        void OnDisposed(object sender, EventArgs e)
        {
            connMgr.BeforeConnectionRemove -= OnBeforeConnectionRemove;
            connMgr.ConnectionAdded -= OnConnectionAdded;
            connMgr.ConnectionRefreshed -= OnConnectionRefreshed;
            connMgr.ConnectionRemoved -= OnConnectionRemoved;
            connMgr.ConnectionRenamed -= OnConnectionRenamed;
        }

        public virtual bool CanClose
        {
            get { return true; }
        }

        public void Close()
        {
            ViewContentClosing(this, EventArgs.Empty);
        }

        public event EventHandler ViewContentClosing;
    }
}
