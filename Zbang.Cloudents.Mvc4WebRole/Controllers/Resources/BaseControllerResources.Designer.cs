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
    internal class BaseControllerResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BaseControllerResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Zbang.Cloudents.Mvc4WebRole.Controllers.Resources.BaseControllerResources", typeof(BaseControllerResources).Assembly);
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
        ///   Looks up a localized string similar to You must start a discussion or post a document.
        /// </summary>
        internal static string FillComment {
            get {
                return ResourceManager.GetString("FillComment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Quiz id cannot be 0.
        /// </summary>
        internal static string QuizController_CreateQuestion_Quiz_id_cannot_be_0 {
            get {
                return ResourceManager.GetString("QuizController_CreateQuestion_Quiz_id_cannot_be_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Answers is requeried.
        /// </summary>
        internal static string QuizController_SaveAnswers_Answers_is_requeried {
            get {
                return ResourceManager.GetString("QuizController_SaveAnswers_Answers_is_requeried", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Whoops... we seem to be experiencing a technical error. Please try again.
        /// </summary>
        internal static string UnspecifiedError {
            get {
                return ResourceManager.GetString("UnspecifiedError", resourceCulture);
            }
        }
    }
}
