﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zbang.Zbox.Domain.CommandHandlers.Resources {
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
    public class CommandHandlerResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CommandHandlerResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Zbang.Zbox.Domain.CommandHandlers.Resources.CommandHandlerResources", typeof(CommandHandlerResources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change user email.
        /// </summary>
        public static string CannotChangeEmail {
            get {
                return ResourceManager.GetString("CannotChangeEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change password.
        /// </summary>
        public static string CannotChangePwd {
            get {
                return ResourceManager.GetString("CannotChangePwd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email is not in the correct format.
        /// </summary>
        public static string EmailNotCorrect {
            get {
                return ResourceManager.GetString("EmailNotCorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This email is taken.
        /// </summary>
        public static string EmailTaken {
            get {
                return ResourceManager.GetString("EmailTaken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User does not exist.
        /// </summary>
        public static string UserNotExist {
            get {
                return ResourceManager.GetString("UserNotExist", resourceCulture);
            }
        }
    }
}
