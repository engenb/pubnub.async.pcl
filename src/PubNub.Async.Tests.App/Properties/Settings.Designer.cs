﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PubNub.Async.Tests.App.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CIPHER_KEY")]
        public string CipherKey {
            get {
                return ((string)(this["CipherKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("pub-c-1ad3ed8a-1ecf-4d26-ba77-b554f077e87e")]
        public string PublishKey {
            get {
                return ((string)(this["PublishKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sub-c-4d29e328-101c-11e6-a8fd-02ee2ddab7fe")]
        public string SubscribeKey {
            get {
                return ((string)(this["SubscribeKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sec-c-NWMyZTE4ZmUtMDlkZC00NzM5LThiYzQtOWJjMTdjNmRmY2Rl")]
        public string SecretKey {
            get {
                return ((string)(this["SecretKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("pub-c-bc275f09-c3c1-4cce-b52e-bea8ff9c5b55")]
        public string PamPublishKey {
            get {
                return ((string)(this["PamPublishKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sub-c-880a28f4-2e5d-11e6-a01f-0619f8945a4f")]
        public string PamSubKey {
            get {
                return ((string)(this["PamSubKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sec-c-Y2JkZDM1MWMtNjQyMy00NjJiLTk5NGQtZWIyMTQ5ZTlmYTk0")]
        public string PamSecKey {
            get {
                return ((string)(this["PamSecKey"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FunctionalTestChannel")]
        public string Channel {
            get {
                return ((string)(this["Channel"]));
            }
        }
    }
}
