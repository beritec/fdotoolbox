﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FdoToolbox.Express {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FdoToolbox.Express.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connect via ODBC.
        /// </summary>
        internal static string CMD_ConnectOdbc {
            get {
                return ResourceManager.GetString("CMD_ConnectOdbc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connect to SDF.
        /// </summary>
        internal static string CMD_ConnectSdf {
            get {
                return ResourceManager.GetString("CMD_ConnectSdf", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connect to SHP.
        /// </summary>
        internal static string CMD_ConnectShp {
            get {
                return ResourceManager.GetString("CMD_ConnectShp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connect to SHP directory.
        /// </summary>
        internal static string CMD_ConnectShpDir {
            get {
                return ResourceManager.GetString("CMD_ConnectShpDir", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SDF file.
        /// </summary>
        internal static string CMD_CreateSdf {
            get {
                return ResourceManager.GetString("CMD_CreateSdf", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SHP file.
        /// </summary>
        internal static string CMD_CreateShp {
            get {
                return ResourceManager.GetString("CMD_CreateShp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bulk Copy.
        /// </summary>
        internal static string CMD_ExpressBcp {
            get {
                return ResourceManager.GetString("CMD_ExpressBcp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save Schema to new SDF.
        /// </summary>
        internal static string CMD_SaveSchemaSdf {
            get {
                return ResourceManager.GetString("CMD_SaveSchemaSdf", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name was empty or already exists. Please pick another.
        /// </summary>
        internal static string ERR_CONNECTION_NAME_EMPTY_OR_EXISTS {
            get {
                return ResourceManager.GetString("ERR_CONNECTION_NAME_EMPTY_OR_EXISTS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SDF files (*.sdf)|*.sdf|SHP Files (*.shp)|*.shp|SQLite (*.db)|*.db|SQLite (*.sqlite)|*.sqlite.
        /// </summary>
        internal static string FILTER_EXPRESS_BCP {
            get {
                return ResourceManager.GetString("FILTER_EXPRESS_BCP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SDF Files (*.sdf)|*.sdf.
        /// </summary>
        internal static string FILTER_SDF {
            get {
                return ResourceManager.GetString("FILTER_SDF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SHP Files (*.shp)|*.shp.
        /// </summary>
        internal static string FILTER_SHP {
            get {
                return ResourceManager.GetString("FILTER_SHP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SDF file created. Create a connection?.
        /// </summary>
        internal static string MSG_CONNECT_SDF {
            get {
                return ResourceManager.GetString("MSG_CONNECT_SDF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SHP file created. Create a connection?.
        /// </summary>
        internal static string MSG_CONNECT_SHP {
            get {
                return ResourceManager.GetString("MSG_CONNECT_SHP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SDF file created.
        /// </summary>
        internal static string MSG_SDF_CREATED {
            get {
                return ResourceManager.GetString("MSG_SDF_CREATED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SHP file created.
        /// </summary>
        internal static string MSG_SHP_CREATED {
            get {
                return ResourceManager.GetString("MSG_SHP_CREATED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter the name of the new connection.
        /// </summary>
        internal static string PROMPT_ENTER_CONNECTION {
            get {
                return ResourceManager.GetString("PROMPT_ENTER_CONNECTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create ODBC Connection.
        /// </summary>
        internal static string TITLE_CONNECT_ODBC {
            get {
                return ResourceManager.GetString("TITLE_CONNECT_ODBC", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SDF Connection.
        /// </summary>
        internal static string TITLE_CONNECT_SDF {
            get {
                return ResourceManager.GetString("TITLE_CONNECT_SDF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SHP Connection.
        /// </summary>
        internal static string TITLE_CONNECT_SHP {
            get {
                return ResourceManager.GetString("TITLE_CONNECT_SHP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SHP directory connection.
        /// </summary>
        internal static string TITLE_CONNECT_SHP_DIR {
            get {
                return ResourceManager.GetString("TITLE_CONNECT_SHP_DIR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection Name.
        /// </summary>
        internal static string TITLE_CONNECTION_NAME {
            get {
                return ResourceManager.GetString("TITLE_CONNECTION_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SDF file.
        /// </summary>
        internal static string TITLE_CREATE_SDF {
            get {
                return ResourceManager.GetString("TITLE_CREATE_SDF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create SHP file.
        /// </summary>
        internal static string TITLE_CREATE_SHP {
            get {
                return ResourceManager.GetString("TITLE_CREATE_SHP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Express Bulk Copy.
        /// </summary>
        internal static string TITLE_EXPRESS_BULK_COPY {
            get {
                return ResourceManager.GetString("TITLE_EXPRESS_BULK_COPY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open File.
        /// </summary>
        internal static string TITLE_OPEN_FILE {
            get {
                return ResourceManager.GetString("TITLE_OPEN_FILE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save File.
        /// </summary>
        internal static string TITLE_SAVE_FILE {
            get {
                return ResourceManager.GetString("TITLE_SAVE_FILE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save schema as SDF.
        /// </summary>
        internal static string TITLE_SAVE_SCHEMA_AS_SDF {
            get {
                return ResourceManager.GetString("TITLE_SAVE_SCHEMA_AS_SDF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        internal static string TXT_CANCEL {
            get {
                return ResourceManager.GetString("TXT_CANCEL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OK.
        /// </summary>
        internal static string TXT_OK {
            get {
                return ResourceManager.GetString("TXT_OK", resourceCulture);
            }
        }
    }
}
