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
using FdoToolbox.Core.ClientServices;
using FdoToolbox.Core.Common;

namespace FdoToolbox.Core
{
    public delegate void ConnectionEventHandler(string name);
    public delegate void ConnectionRenamedEventHandler(string oldName, string newName);
    public delegate void ConnectionBeforeRemoveHandler(string name, ref bool cancel);

    public interface ISpatialConnectionMgr : IDisposable
    {
        string CreateUniqueName();
        void AddConnection(string name, IConnection conn);
        void RemoveConnection(string name);
        FdoConnectionInfo GetConnection(string name);
        ICollection<string> GetConnectionNames();
        void RenameConnection(string oldName, string newName);
        bool CanRenameConnection(string oldName, string newName, ref string reason);

        event ConnectionBeforeRemoveHandler BeforeConnectionRemove;
        event ConnectionEventHandler ConnectionAdded;
        event ConnectionEventHandler ConnectionRemoved;
        event ConnectionRenamedEventHandler ConnectionRenamed;
    }
}