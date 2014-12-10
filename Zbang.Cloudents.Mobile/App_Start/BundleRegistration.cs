using System.Collections.Generic;
using Zbang.Cloudents.Mvc4WebRole;

namespace Zbang.Cloudents.Mobile
{
    public class BundleRegistration
    {
        public static void RegisterBundles()
        {
            BundleConfig.RegisterBundle(RegisterCss(), RegisterJs());
        }

        private static IDictionary<string, IEnumerable<JsFileWithCdn>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>();

            jsDictionary.Add("homeMobile", new[] {
              new JsFileWithCdn("~/Js/Mobile/Logon.js"),
              new JsFileWithCdn("~/Js/Mobile/Welcome.js")
            });

            jsDictionary.Add("home", new[]
            {
                new JsFileWithCdn("~/Scripts/validatinator.min.js"),
                new JsFileWithCdn("~/Js/staticShim.js"),
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/HomePage.js")
            });

            jsDictionary.Add("General", new[]
            {
                new JsFileWithCdn("~/scripts/jquery-2.1.1.js",
                    "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                //new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
                //new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
                //new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                new JsFileWithCdn("~/scripts/Modernizr.js"),

                //new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                new JsFileWithCdn("~/Js/Utils2.js"),
                new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

                new JsFileWithCdn("~/Js/pubsub2.js"),
                new JsFileWithCdn("~/scripts/svg4everybody.js")
                //new JsFileWithCdn("~/Js/externalScriptsInitializer.js")
            });


            jsDictionary.Add("mobile", new[] {
                  new JsFileWithCdn("~/Scripts/jquery-2.1.1.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.js"),
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.js"),


                new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
                new JsFileWithCdn("~/Scripts/Modernizr.js"),

                 new JsFileWithCdn("~/Scripts/plupload/plupload.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.html4.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.html5.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.flash.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.silverlight.js"),
                new JsFileWithCdn("~/Scripts/jquery-ui-1.10.4.min.js"),
                new JsFileWithCdn("~/Scripts/elasticTextBox.js"),

                new JsFileWithCdn("~/Js/Utils2.js"),
                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                 new JsFileWithCdn("~/Js/pubsub2.js"),
                new JsFileWithCdn("~/Js/Cache2.js"),
                new JsFileWithCdn("~/Js/DataContext2.js"),

                new JsFileWithCdn("~/Js/Navigation2.js"),
                new JsFileWithCdn("~/Js/Mobile/MBaseViewModel.js"),
                new JsFileWithCdn("~/Js/Mobile/MInvite.js"),
                new JsFileWithCdn("~/Js/Mobile/MUpload.js"),
                //invite
                new JsFileWithCdn("~/Js/Mobile/MInviteViewModel.js"),
                //wall
                new JsFileWithCdn("~/Js/Mobile/MWallViewModel.js"),
                //library
                new JsFileWithCdn("~/Js/Mobile/MLibrary.js"),
                //new JsFileWithCdn("/Js/Mobile/MLibraryChoose.js"),
                //dashboard
                new JsFileWithCdn("~/Js/Mobile/MBoxesViewModel.js"),

                //box
                new JsFileWithCdn("~/Js/Mobile/MBoxViewModel.js"),
                new JsFileWithCdn("~/Js/Mobile/MBoxItemsViewModel.js"),
                // new JsFileWithCdn("~/Js/Mobile/MCommentsViewModel.js"),

                //Account settings
                 new JsFileWithCdn("~/Js/Mobile/MAccountSettings.js")
                });
            return jsDictionary;
        }

        private static IDictionary<string, IEnumerable<string>> RegisterCss()
        {
            var cssDictionary = new Dictionary<string, IEnumerable<string>>();

            cssDictionary.Add("siteMobile", new[] {
                "~/Content/Normalize.css", 
                "~/Content/General.css",
                "~/Content/SiteMobile.css"});
            cssDictionary.Add("siteMobileRtl", new[] {
                "~/Content/GeneralRtl.css",
                "~/Content/SiteMobileRtl.css"
                });

            cssDictionary.Add("itemMobile", new[] { "~/Content/ItemMobile.css" });
            cssDictionary.Add("itemMobileRtl", new[] { "~/Content/ItemMobileRtl.css" });

            cssDictionary.Add("mobile",new[] {
                "~/Content/Normalize.css", "~/Content/General.css", "~/Content/Mobile.css"});
            cssDictionary.Add("mobileRtl", new[] { "~/Content/GeneralRtl.css", "~/Content/MobileRtl.css"});


            return cssDictionary;
        }
    }
}