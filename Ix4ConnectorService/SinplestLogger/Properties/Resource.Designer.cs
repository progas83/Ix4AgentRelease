﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SinplestLogger.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SinplestLogger.Properties.Resource", typeof(Resource).Assembly);
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
        ///   Looks up a localized string similar to &lt;div class=&quot;rTableRow&quot;&gt; 
        ///			 &lt;div class=&quot;rTableCell&quot; &gt;ContentName:&lt;/div&gt; 
        ///			 &lt;div class=&quot;rTableCell2&quot; width=&quot;900&quot; &gt;ContentText&lt;/div&gt;
        ///			 &lt;/div&gt;.
        /// </summary>
        internal static string ContentDescription {
            get {
                return ResourceManager.GetString("ContentDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html PUBLIC &quot;-//W3C//DTD XHTML 1.0 Transitional//EN&quot; &quot;http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd&quot;&gt;
        ///&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        ///&lt;head&gt;
        ///&lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=utf-8&quot; /&gt;
        ///&lt;title&gt;Untitled Document&lt;/title&gt;
        ///&lt;style type=&quot;text/css&quot;&gt;
        ///body {
        ///	font-family: &quot;Segoe UI&quot;;
        ///	font-size: 14px;
        ///	color: #3d3f3a;
        ///	background-color: #cccccc;
        ///	background-repeat: repeat;
        ///	text-align: left;
        ///	}
        ///h1{
        ///color: #000;
        ///}
        ///text{
        ///color: #000;
        ///}
        ///tr.bor [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MailReportExceptionsHtml {
            get {
                return ResourceManager.GetString("MailReportExceptionsHtml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;tr class=&quot;border_bottom&quot; &gt;
        ///    &lt;td align=&quot;center&quot; style=&quot;font-weight:bold&quot; width=&quot;40&quot;&gt;RN&lt;/td&gt;
        ///    &lt;td &gt;
        ///		ContentDescription
        ///    &lt;/td&gt;
        ///&lt;/tr&gt;.
        /// </summary>
        internal static string ReportContent {
            get {
                return ResourceManager.GetString("ReportContent", resourceCulture);
            }
        }
    }
}
