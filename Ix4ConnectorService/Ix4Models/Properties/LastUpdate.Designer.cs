﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ix4Models.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    public sealed partial class LastUpdate : global::System.Configuration.ApplicationSettingsBase {
        
        private static LastUpdate defaultInstance = ((LastUpdate)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new LastUpdate())));
        
        public static LastUpdate Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public long Orders {
            get {
                return ((long)(this["Orders"]));
            }
            set {
                this["Orders"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public long Deliveries {
            get {
                return ((long)(this["Deliveries"]));
            }
            set {
                this["Deliveries"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public Ix4Models.EDLastUpdate ExportData {
            get {
                return ((Ix4Models.EDLastUpdate)(this["ExportData"]));
            }
            set {
                this["ExportData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1481621222")]
        public long Articles {
            get {
                return ((long)(this["Articles"]));
            }
            set {
                this["Articles"] = value;
            }
        }
    }
}