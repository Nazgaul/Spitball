using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using SquishIt.Framework.JavaScript;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class BundleConfig
    {
        private static readonly Dictionary<string, string> CssBundels = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> JsBundels = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> JsRemoteBundles = new Dictionary<string, string>();
        private static readonly string CdnLocation = GetValueFromCloudConfig();

        public static string CssLink(string key)
        {
            string retVal;
            CssBundels.TryGetValue(key, out retVal);

            return retVal;
        }
        public static string JsLink(string key)
        {
            return JsBundels[key];
        }

        public static string JsRemoteLinks(string key)
        {
            return JsRemoteBundles[key];

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
            RegisterCss("lang.he-IL", "~/Content/lang.he-IL.css");

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
                "~/Content/DashLib.css",
                "~/Content/jquery.mCustomScrollbar.css");

            RegisterCss("staticRtl", "~/Content/GeneralRtl.css",
                "~/Content/StaticRtl.css",
                "~/Content/HeaderFooterRtl.css");

            RegisterCss("static", "~/Content/General.css",
                "~/Content/HeaderFooter.css",
                "~/Content/Animations.css",
                "~/Content/Static.css");

            //RegisterCss("welcome", "~/Content/Welcome.css");
            //RegisterCss("welcomeRtl", "~/Content/WelcomeRtl.css");


            RegisterCss("shoppingRtl", "~/Content/GeneralRtl.css",
                "~/Content/ShopppingRtl.css");

            RegisterCss("shopping", "~/Content/General.css",
                "~/Content/Shopping.css");


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

            //test
            //RegisterJsRegular("angular",
            //    new JsFileWithCdn("~/Scripts/angular.js", "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.17/angular.min.js"),
            //    new JsFileWithCdn("~/Scripts/angular-route.js", "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.17/angular-route.min.js"),
            //  new JsFileWithCdn("~/Scripts/angular-cache-2.3.4.js"),
            //  new JsFileWithCdn("~/Scripts/ui-bootstrap-custom-tpls-0.10.0.min.js"),
            //    new JsFileWithCdn("~/Scripts/elastic.js"),
            //  new JsFileWithCdn("~/Scripts/angular-mcustomscrollbar.js"),
            //    new JsFileWithCdn("~/Js/app/services.js"),
            //     new JsFileWithCdn("~/Js/app/filters.js"),
            //    new JsFileWithCdn("~/Js/app/directives.js"),
            //    new JsFileWithCdn("~/Scripts/ng-modal.js"),
            //     new JsFileWithCdn("~/Js/app/dashboardcontroller.js"),
            //     new JsFileWithCdn("~/Js/app/boxcontroller.js"),
            //    new JsFileWithCdn("~/Js/app/quizcreatecontroller.js"),
            //     new JsFileWithCdn("~/Js/app/app.js")

            // );js/ItemViewModel4
            //RegisterJsRoutes("R_App", "/Js/app.js");

            RegisterJsRoutes("R_All",
                "/js/app.js",
                "/js/routes",
                "/js/services/dependencyResolverFor.js",
                "/js/directives/ngPlaceholder.js",
                "/js/directives/mLoader.js");

            RegisterJsRoutes("R_OldAll",
                          "/Scripts/externalScriptLoader.js",
                "/js/Utils.js",
                "/js/pubsub.js",
                "/js/DataContext.js",
                "/js/Dialog.js",
                "/js/GenericEvents.js",
                "/js/Cache.js");

            RegisterJsRoutes("R_Box", 
                "/js/services/dropbox.js",
                "/Scripts/draganddrop.js",
                "/js/filters/trustedHtml.js",
                "/Js/services/qna.js",
                "/Js/services/upload.js",
                "/Js/controllers/box/boxCtrl.js",
                "/Js/controllers/box/tabCtrl.js",
                "/Js/controllers/box/qnaCtrl.js",
                "/Js/controllers/box/uploadCtrl.js",
                "/Js/controllers/box/manageCtrl.js"
                );
            RegisterJsRoutes("R_DashboardBox",
                "/js/services/newUpdates.js",
                "/js/services/box.js"
                );
            RegisterJsRoutes("R_BoxItem",
                "/Js/services/item.js"

                );
            RegisterJsRoutes("R_BoxQuiz", 
                "/Js/services/quiz.js"
                );

            RegisterJsRoutes("R_Dashboard", "/Js/services/dashboard.js",
                
                "/js/services/user.js",
                
                "/js/filters/actionText.js",
                "/js/filters/orderBy.js",
                "/js/controllers/dashboard/createBoxCtrl.js",
                "/js/controllers/dashboard/showFriendsCtrl.js",
                "/js/controllers/dashboard/dashboardCtrl.js");


            RegisterJsRoutes("R_Library",
                "/Scripts/externalScriptLoader.js",
                "/js/Utils.js",
                "/js/pubsub.js",
                "/js/DataContext.js",
                "/js/Dialog.js",
                "/js/GenericEvents.js",
                "/js/controllers/library/libraryCtrl.js",
                "/js/Library.js",
                "/js/Cache.js");

            RegisterJsRoutes("R_Item",
                "/js/controllers/item/itemCtrl.js",
                "js/ItemViewModel4"
                );

            RegisterJsRegular("home",
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/Welcome.js"));
            RegisterJsRegular("homeMobile",
                new JsFileWithCdn("~/Js/Mobile/Logon.js"),
                new JsFileWithCdn("~/Js/Mobile/Welcome.js"));

            RegisterJsRegular("ChooseLib",
                //new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Js/Cache.js"),
                new JsFileWithCdn("~/Js/DataContext.js"),
                new JsFileWithCdn("~/Js/GenericEvents.js"),
                new JsFileWithCdn("~/Js/LibraryChoose.js"));

            RegisterJsRegular("MChooseLib",
                new JsFileWithCdn("~/Js/Mobile/MLibraryChoose.js"));


            RegisterJsRegular("General",
                new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.js"),// the script is too small
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                new JsFileWithCdn("~/Scripts/Modernizr.js"),

                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                //new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                new JsFileWithCdn("~/Js/Utils.js"),
                new JsFileWithCdn("~/Js/pubsub.js"),

                new JsFileWithCdn("~/Js/GmfcnHandler.js")
                //new JsFileWithCdn("~/Js/externalScriptsInitializer.js")
                );
            RegisterJsRegular("faq", new JsFileWithCdn("~/Js/externalScriptsInitializer.js"));


            #region layout3
            RegisterJsRegular("cd1",
                new JsFileWithCdn("~/Scripts/jquery-ui-1.10.4.min.js"),
                new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Scripts/knockout-delegatedEvents.js"),
                new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
                new JsFileWithCdn("~/Scripts/jquery.slimscroll.js"),

                new JsFileWithCdn("~/Scripts/elasticTextBox.js"),
                                new JsFileWithCdn("~/Scripts/jquery.mousewheel.min.js"),

                                new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.js"),

                //new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.concat.min.js"),
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
                //new JsFileWithCdn("~/Js/BoxesViewModel.js"),
                //new JsFileWithCdn("~/Js/DashboardAside.js"),
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

            RegisterJsRegular("library",
                new JsFileWithCdn("~/Js/Library.js"));
            //new JsFileWithCdn("~/Js/LibraryChoose.js"));

            //RegisterJs("box",
            //     new JsFileWithCdn("~/Js/BoxViewModel2.js"),
            //    new JsFileWithCdn("~/Js/BoxItemsViewModel2.js"),
            //    new JsFileWithCdn("~/Js/Upload2.js"),
            //    new JsFileWithCdn("~/Js/Invite.js"),
            RegisterJsRegular("item",
                //new JsFileWithCdn("~/Scripts/jquery.dotdotdot.min.js"),
                new JsFileWithCdn("~/Js/ItemViewModel4.js"));
            #endregion layout3


            #region mobile
            RegisterJsRegular("mobileItem", new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                                    new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                                    new JsFileWithCdn("~/Js/Utils.js"),
                                    new JsFileWithCdn("~/Js/pubsub.js"),
                                    new JsFileWithCdn("~/Js/Cache.js"),
                                    new JsFileWithCdn("~/Js/DataContext.js"),
                                    new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js"));
            RegisterJsRegular("mobile",
                  new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"),
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
            //CopyFilesToCdn("/Content/Fonts", "*.eot");
            //CopyFilesToCdn("/Content/Fonts", "*.svg");
            //CopyFilesToCdn("/Content/Fonts", "*.ttf");
            //CopyFilesToCdn("/Content/Fonts", "*.woff");

        }

        public static bool IsDebugEnabled()
        {
            return HttpContext.Current.Request.IsLocal || string.IsNullOrWhiteSpace(CdnLocation);
        }

        private static void RegisterCss(string key, params string[] cssFiles)
        {
            var cssbundle = SquishIt.Framework.Bundle.Css();
            cssbundle.WithReleaseFileRenderer(new SquishItRenderer());
            foreach (var cssFile in cssFiles)
            {
                cssbundle.Add(cssFile);
            }

            var cdnUrl = CdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                cssbundle.WithOutputBaseHref(cdnUrl);
                CssBundels.Add(key, cssbundle.Render("~/gzip/c#.css"));
                CopyFilesToCdn("~/gzip/", "*.css", SearchOption.TopDirectoryOnly);
            }
            else
            {
                CssBundels.Add(key, cssbundle.Render("~/cdn/gzip/c#.css"));
            }


        }

        private static void RegisterJsRoutes(string key, params string[] jsFiles)
        {
            JsRemoteBundles.Add(key, RegisterJs(jsFiles.Select(s => new JsFileWithCdn(s)), new JavaScriptBundleImp()));
        }

        private static string RegisterJs(IEnumerable<JsFileWithCdn> jsFiles, JavaScriptBundle javaScriptBundleImp)
        {
            var jsBundle = javaScriptBundleImp;
            jsBundle.WithReleaseFileRenderer(new SquishItRenderer());
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

            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {

                jsBundle.WithOutputBaseHref(cdnUrl);
                return jsBundle.Render("~/gzip/j#.js");
            }
            return jsBundle.Render("~/cdn/gzip/j#.js");
        }

        private static void RegisterJsRegular(string key, params JsFileWithCdn[] jsFiles)
        {
            JsBundels.Add(key, RegisterJs(jsFiles, SquishIt.Framework.Bundle.JavaScript()));
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
                    var directory = Path.GetDirectoryName(cdnFilePath);
                    if (directory != null) Directory.CreateDirectory(directory);
                    File.Copy(filePath, Path.Combine(cdnRoot, relativePath));
                }
                catch (Exception ex)
                {
                    Zbox.Infrastructure.Trace.TraceLog.WriteError(string.Format("On Copy Files to cdn filePath: {0} cdnFilePath: {1}", filePath, cdnFilePath), ex);
                }
            }
        }

        class JsFileWithCdn
        {
            public JsFileWithCdn(string localFile)
            {
                LocalFile = localFile;
            }
            // ReSharper disable once UnusedMember.Local
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