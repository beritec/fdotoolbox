using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;
using FdoToolbox.Core.Utility;

//Type aliases to save typing
using Res = ICSharpCode.Core.ResourceService;
using Msg = ICSharpCode.Core.MessageService;
using FdoToolbox.Express.Controls;
using FdoToolbox.Base;

namespace FdoToolbox.Express.Commands
{
    public class ConnectSdfCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string file = FileService.OpenFile(Res.GetString("TITLE_CONNECT_SDF"), Res.GetString("FILTER_SDF"));
            if (FileService.FileExists(file))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SDF", file);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                NamingService namer = ServiceManager.Instance.GetService<NamingService>();

                string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), namer.GetDefaultConnectionName("OSGeo.SDF"));
                if (name == null)
                    return;
                
                while(name == string.Empty || mgr.NameExists(name))
                {
                    Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                    name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                    if (name == null)
                        return;
                }
                mgr.AddConnection(name, conn);
            }
        }
    }

    public class ConnectShpCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            string file = FileService.OpenFile(Res.GetString("TITLE_CONNECT_SHP"), Res.GetString("FILTER_SHP"));
            if (FileService.FileExists(file))
            {
                FdoConnection conn = ExpressUtility.CreateFlatFileConnection("OSGeo.SHP", file);
                FdoConnectionManager mgr = ServiceManager.Instance.GetService<FdoConnectionManager>();
                NamingService namer = ServiceManager.Instance.GetService<NamingService>();

                string name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), namer.GetDefaultConnectionName("OSGeo.SHP"));
                if (name == null)
                    return;

                while (string.IsNullOrEmpty(name) || mgr.NameExists(name))
                {
                    Msg.ShowError(Res.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS"));
                    name = Msg.ShowInputBox(Res.GetString("TITLE_CONNECTION_NAME"), Res.GetString("PROMPT_ENTER_CONNECTION"), name);

                    if (name == null)
                        return;
                }
                mgr.AddConnection(name, conn);
            }
        }
    }

    public class ConnectOdbcCommand : AbstractMenuCommand 
    {
        public override void Run()
        {
            ConnectOdbcCtl ctl = new ConnectOdbcCtl();
            Workbench wb = Workbench.Instance;
            wb.ShowContent(ctl, ViewRegion.Dialog);
        }
    }
}