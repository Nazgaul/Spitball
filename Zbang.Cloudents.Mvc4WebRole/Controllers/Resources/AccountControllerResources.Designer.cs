﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zbang.Cloudents.Mvc4WebRole.Controllers.Resources {
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
    internal class AccountControllerResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccountControllerResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Zbang.Cloudents.Mvc4WebRole.Controllers.Resources.AccountControllerResources", typeof(AccountControllerResources).Assembly);
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
        ///   Looks up a localized string similar to something went wrong, try again later.
        /// </summary>
        internal static string AccountController_PasswordUpdate_Error {
            get {
                return ResourceManager.GetString("AccountController_PasswordUpdate_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect code.
        /// </summary>
        internal static string ChangeEmailCodeError {
            get {
                return ResourceManager.GetString("ChangeEmailCodeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This email address does not exist.
        /// </summary>
        internal static string EmailDoesNotExists {
            get {
                return ResourceManager.GetString("EmailDoesNotExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There seems to be a problem getting data from Facebook.
        /// </summary>
        internal static string FacebookGetDataError {
            get {
                return ResourceManager.GetString("FacebookGetDataError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have signed up to Spitball using Facebook. To log in, go to the homepage and click on the Facebook button.
        /// </summary>
        internal static string FbRegisterError {
            get {
                return ResourceManager.GetString("FbRegisterError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have signed up to Spitball using Google. To log in, go to the homepage and click on the Google button.
        /// </summary>
        internal static string GoogleForgotPasswordError {
            get {
                return ResourceManager.GetString("GoogleForgotPasswordError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The user name or password provided is incorrect.
        /// </summary>
        internal static string LogonError {
            get {
                return ResourceManager.GetString("LogonError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Whoops, we seem to be experiencing a technical error. Please try again.
        /// </summary>
        internal static string UnspecifiedError {
            get {
                return ResourceManager.GetString("UnspecifiedError", resourceCulture);
            }
        }
    }
}
