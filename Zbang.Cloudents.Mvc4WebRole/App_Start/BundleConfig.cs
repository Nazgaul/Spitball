using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Zbang.Cloudents.Mvc4WebRole.Helpers.Mimify;

namespace Zbang.Cloudents.Mvc4WebRole.App_Start
{
    public static class BundleConfig
    {
        private static readonly Dictionary<string, string> CssBundels = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> JsBundels = new Dictionary<string, string>();
        private static readonly string CdnLocation = GetValueFromCloudConfig();

        public static string CssLink(string key)
        {
            var retVal = string.Empty;
            CssBundels.TryGetValue(key, out retVal);

            return retVal;
        }
        public static string JsLink(string key)
        {
            return JsBundels[key];
        }

        public static string CdnEndpointUrl
        {
            get
            {
                return CdnLocation;
            }
        }

        public static void RegisterBundle()
        {
            #region Css
            RegisterCss("lang.ru-RU", "~/Content/lang.ru-RU.css");

            RegisterCss("newrtl3", "~/Content/GeneralRtl.css",
                "~/Content/rtl3.css",
                "~/Content/HeaderFooterRtl.css");

            RegisterCss("newcore3", "~/Content/General.css",
                "~/Content/HeaderFooter.css",
                "~/Content/Site3.css",
                "~/Content/AccountInfo.css",
                "~/Content/Animations.css",
                "~/Content/UserPage.css",
                "~/Content/Search.css",
                "~/Content/Sidebar.css",
                "~/Content/Modal.css",
                "~/Content/QnA.css",
                "~/Content/Quiz.css",
                "~/Content/Invite.css",
                "~/Content/Upload.css",
                "~/Content/Box3.css",
                "~/Content/Item3.css",
                "~/Content/Settings.css",
                "~/Content/DashLib.css");

            RegisterCss("staticRtl", "~/Content/GeneralRtl.css",
                "~/Content/StaticRtl.css",
                "~/Content/HeaderFooterRtl.css");

            RegisterCss("static", "~/Content/General.css",
                "~/Content/HeaderFooter.css",
                "~/Content/Animations.css",
                "~/Content/Static.css");

            //RegisterCss("welcome", "~/Content/Welcome.css");
            //RegisterCss("welcomeRtl", "~/Content/WelcomeRtl.css");



            RegisterCss("home", "~/Content/General.css",
                "~/Content/Home.css");

            RegisterCss("homeRtl", "~/Content/GeneralRtl.css",
                "~/Content/HomeRtl.css");

            RegisterCss("mobile", "~/Content/General.css", "~/Content/Mobile.css");
            RegisterCss("mobileRtl", "~/Content/GeneralRtl.css", "~/Content/MobileRtl.css");

            RegisterCss("siteMobile", "~/Content/General.css", "~/Content/SiteMobile.css");
            RegisterCss("siteMobileRtl", "~/Content/GeneralRtl.css",
                "~/Content/SiteMobileRtl.css");

            RegisterCss("itemMobile", "~/Content/ItemMobile.css");
            RegisterCss("itemMobileRtl", "~/Content/ItemMobileRtl.css");
            #endregion
            RegisterJs("home",
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/Welcome.js"));
            RegisterJs("homeMobile",
                new JsFileWithCdn("~/Js/Mobile/Logon.js"),
                new JsFileWithCdn("~/Js/Mobile/Welcome.js"));

            RegisterJs("ChooseLib",
                  new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Js/Cache.js"),
                new JsFileWithCdn("~/Js/DataContext.js"),
                new JsFileWithCdn("~/Js/LibraryChoose.js"));

            RegisterJs("MChooseLib",
                new JsFileWithCdn("~/Js/Mobile/MLibraryChoose.js"));


            RegisterJs("General",
                   new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.js"),// the script is too small
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                new JsFileWithCdn("~/Scripts/Modernizr.js"),

                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                new JsFileWithCdn("~/Js/Utils.js"),
                new JsFileWithCdn("~/Js/pubsub.js"),

                new JsFileWithCdn("~/Js/GmfcnHandler.js")
                //new JsFileWithCdn("~/Js/externalScriptsInitializer.js")

                );
            RegisterJs("faq", new JsFileWithCdn("~/Js/externalScriptsInitializer.js"));
                

            #region layout3
            RegisterJs("cd1",
                new JsFileWithCdn("~/Scripts/jquery-ui-1.10.4.min.js"),
                new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Scripts/knockout-delegatedEvents.js"),
                new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
                new JsFileWithCdn("~/Scripts/jquery.slimscroll.js"),
                //new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.min.js"),
                //new JsFileWithCdn("~/Scripts/jquery.mousewheel.js"),
                new JsFileWithCdn("~/Scripts/elasticTextBox.js"),

                new JsFileWithCdn("~/Scripts/plupload/plupload.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.html4.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.html5.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.flash.js"),
                //new JsFileWithCdn("~/Scripts/plupload/plupload.silverlight.js"),
                // new JsFileWithCdn("~/Scripts/ZeroClipboard.js"),
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/Cache.js"),
                new JsFileWithCdn("~/Js/DataContext.js"),
                new JsFileWithCdn("~/Js/GenericEvents.js"),
                new JsFileWithCdn("~/Js/Navigation.js"),


                new JsFileWithCdn("~/Js/Dialog.js"), //dialog message
                new JsFileWithCdn("~/Js/Autocomplete.js"), //dialog message
                new JsFileWithCdn("~/Js/Tooltip.js"), //dialog message


               new JsFileWithCdn("~/Js/TooltipGuide.js"),
                
                //new JsFileWithCdn("~/Js/bootstrapper2.js"),
                new JsFileWithCdn("~/Js/Statistics.js"),

                //header
                //new JsFileWithCdn("~/Js/InviteViewModel2.js"),
                new JsFileWithCdn("~/Js/NotificationsViewModel.js"),

                //dashboard page
                new JsFileWithCdn("~/Js/BoxesViewModel.js"),
                new JsFileWithCdn("~/Js/DashboardAside.js"),
                //library page
                new JsFileWithCdn("~/Js/Library.js"),


                //box Page
                new JsFileWithCdn("~/Js/BoxViewModel2.js"),
                new JsFileWithCdn("~/Js/BoxItemsViewModel2.js"),
                new JsFileWithCdn("~/Js/BoxSettings.js"),
                new JsFileWithCdn("~/Js/Upload2.js"),
                new JsFileWithCdn("~/Js/Invite.js"),
                new JsFileWithCdn("/Js/Share.js"),
                new JsFileWithCdn("~/Js/QnA.js"),

                //account settings
                new JsFileWithCdn("~/Js/AccountSettings.js"),

                //item
                new JsFileWithCdn("~/Js/ItemViewModel4.js"),

                //signalR
                new JsFileWithCdn("~/Js/RT.js"),

                //Social
                new JsFileWithCdn("~/Js/SocialConnect.js"),

                //User Page
                new JsFileWithCdn("~/Scripts/CountUp.js"),
                new JsFileWithCdn("~/Js/User.js"),

                //Search page
                new JsFileWithCdn("~/Js/SearchDropdown.js"),
                new JsFileWithCdn("~/Js/Search.js"),

                //Quiz
                new JsFileWithCdn("~/Js/QuizCreate.js"),
                new JsFileWithCdn("~/Js/QuizViewModel.js"),
                new JsFileWithCdn("~/Scripts/stopwatch.js")
                );



            //RegisterJs("dashboard",
            //    new JsFileWithCdn("~/Js/BoxesViewModel.js"),
            //    new JsFileWithCdn("~/Js/DashboardAside.js"));

            //RegisterJs("library",
            //     new JsFileWithCdn("~/Js/Library.js"),
            //    new JsFileWithCdn("~/Js/LibraryChoose.js"));

            //RegisterJs("box",
            //     new JsFileWithCdn("~/Js/BoxViewModel2.js"),
            //    new JsFileWithCdn("~/Js/BoxItemsViewModel2.js"),
            //    new JsFileWithCdn("~/Js/Upload2.js"),
            //    new JsFileWithCdn("~/Js/Invite.js"),
            RegisterJs("item",
                //new JsFileWithCdn("~/Scripts/jquery.dotdotdot.min.js"),
                new JsFileWithCdn("~/Js/ItemViewModel4.js"));
            #endregion layout3


            #region mobile
            RegisterJs("mobileItem", new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                                    new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                                    new JsFileWithCdn("~/Js/Utils.js"),
                                    new JsFileWithCdn("~/Js/pubsub.js"),
                                    new JsFileWithCdn("~/Js/Cache.js"),
                                    new JsFileWithCdn("~/Js/DataContext.js"),
                                    new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js"));
            RegisterJs("mobile",
                  new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.js"),
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.js"),

                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),

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

                new JsFileWithCdn("~/Js/Utils.js"),
                 new JsFileWithCdn("~/Js/pubsub.js"),
                new JsFileWithCdn("~/Js/Cache.js"),
                new JsFileWithCdn("~/Js/DataContext.js"),

                new JsFileWithCdn("~/Js/Navigation.js"),
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
                new JsFileWithCdn("~/Js/Mobile/MCommentsViewModel.js"),

                //Account settings
                 new JsFileWithCdn("~/Js/Mobile/MAccountSettings.js")
                //item
                //new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js")
            );
            //RegisterJs("AccountSettings",
            //    new JsFileWithCdn("~/Js/Mobile/MAccountSettings.js")
            //    );


            #endregion
            CopyFilesToCdn("/Content", "*.min.css");
            CopyFilesToCdn("/Content", "*.png");
            CopyFilesToCdn("/Content", "*.jpg");
            CopyFilesToCdn("/Content", "*.gif");
            CopyFilesToCdn("/Images", "*.*");
            CopyFilesToCdn("/Content/Fonts", "*.eot");
            CopyFilesToCdn("/Content/Fonts", "*.svg");
            CopyFilesToCdn("/Content/Fonts", "*.ttf");
            CopyFilesToCdn("/Content/Fonts", "*.woff");

        }

        public static bool IsDebugEnabled()
        {
            return HttpContext.Current.Request.IsLocal || string.IsNullOrWhiteSpace(CdnLocation);
        }

        private static void RegisterCss(string key, params string[] cssFiles)
        {
            var cssbundle = SquishIt.Framework.Bundle.Css();

            foreach (var cssFile in cssFiles)
            {
                cssbundle.Add(cssFile);
            }

            var cdnUrl = CdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                cssbundle.WithOutputBaseHref(cdnUrl);
                CssBundels.Add(key, cssbundle.Render("~/c#.css"));
                CopyFilesToCdn("~/", "*.css", SearchOption.TopDirectoryOnly);
            }
            else
            {

                CssBundels.Add(key, cssbundle.Render("~/cdn/c#.css"));
            }

        }

        private static void RegisterJs(string key, params JsFileWithCdn[] jsFiles)
        {
            var jsBundle = SquishIt.Framework.Bundle.JavaScript();
            jsBundle.ForceDebug();
            foreach (var jsFile in jsFiles)
            {
                if (string.IsNullOrWhiteSpace(jsFile.CdnFile))
                {
                    if (jsFile.LocalFile.Contains("min"))
                    {
                        jsBundle.AddMinified(jsFile.LocalFile);
                    }
                    else
                    {
                        jsBundle.Add(jsFile.LocalFile);
                    }
                }
                else
                {
                    jsBundle.AddRemote(jsFile.LocalFile, jsFile.CdnFile);
                }
            }
            var cdnUrl = CdnLocation;

            //jsBundle.WithAttribute("Async", string.Empty);
            //jsBundle.WithoutTypeAttribute();
            //            jsBundle.WithDeferredLoad();

            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {

                jsBundle.WithOutputBaseHref(cdnUrl);
                JsBundels.Add(key, jsBundle.Render("~/j#.js"));
                CopyFilesToCdn("~/", "*.js", SearchOption.TopDirectoryOnly);
            }
            else
            {
                JsBundels.Add(key, jsBundle.Render("~/cdn/j#.js"));
            }
        }

        private static string GetValueFromCloudConfig()
        {
            if (!RoleEnvironment.IsAvailable)
            {
                return string.Empty;
            }
            try
            {
                return RoleEnvironment.GetConfigurationSettingValue("CdnEndpoint");
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }


        private static void CopyFilesToCdn(string directoryRelativePath, string fileSearchOption, SearchOption options = SearchOption.AllDirectories)
        {
            var server = HttpContext.Current.Server;
            var appRoot = server.MapPath("~/");
            var cdnRoot = server.MapPath("~/cdn");

            var filesPath = Directory.GetFiles(server.MapPath(directoryRelativePath), fileSearchOption, options);

            foreach (var filePath in filesPath)
            {
                var cdnFilePath = string.Empty;
                try
                {
                    var relativePath = filePath.Replace(appRoot, string.Empty);
                    cdnFilePath = Path.Combine(cdnRoot, relativePath);
                    if (File.Exists(Path.Combine(cdnRoot, relativePath)))
                    {
                        continue;
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(cdnFilePath));
                    File.Copy(filePath, Path.Combine(cdnRoot, relativePath), true);
                }
                catch (Exception ex)
                {
                    Zbang.Zbox.Infrastructure.Trace.TraceLog.WriteError(string.Format("On Copy Files to cdn filePath: {0} cdnFilePath: {1}", filePath, cdnFilePath), ex);
                }
            }
        }

        class JsFileWithCdn
        {
            public JsFileWithCdn(string localFile)
            {
                LocalFile = localFile;
            }
            public JsFileWithCdn(string localFile, string cdnFile)
            {
                LocalFile = localFile;
                CdnFile = cdnFile;
            }
            public string LocalFile { get; private set; }
            public string CdnFile { get; private set; }
        }

    }
}