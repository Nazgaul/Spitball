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

        //public static string JsRemoteLinks(string key)
        //{
        //    return JsRemoteBundles[key];

        //}




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
                "~/Content/ShoppingRtl.css");

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

            RegisterJsRegular("angular-general",
                                new JsFileWithCdn("~/scripts/jquery-2.1.0.min.js",
                    "https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"),

                new JsFileWithCdn("~/scripts/angular.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular.min.js"),

                new JsFileWithCdn("~/scripts/angular-route.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular-route.min.js"),                   
                    new JsFileWithCdn("~/scripts/angular-cookies.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular-cookies.min.js"),
                    
            new JsFileWithCdn("~/scripts/angular-sanitize.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular-sanitize.min.js"),

                    new JsFileWithCdn("~/scripts/angular-animate.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.18/angular-animate.min.js"),

                                    new JsFileWithCdn("~/scripts/stacktrace.js")

                                    );

            RegisterJsRegular("angular-layout3",

                new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),
                new JsFileWithCdn("~/scripts/bindonce.js"),
                //new JsFileWithCdn("~/Scripts/angular-cache-2.3.4.js"),
                new JsFileWithCdn("~/scripts/uiBootstrapTpls0.11.0.js"),
                new JsFileWithCdn("~/scripts/angular-draganddrop.js"),
                new JsFileWithCdn("~/scripts/angular-debounce.js"),
                new JsFileWithCdn("~/scripts/jquery.mousewheel.min.js"),
                new JsFileWithCdn("~/scripts/jquery.mCustomScrollbar.js"),
                new JsFileWithCdn("~/scripts/Modernizr.js"),
                new JsFileWithCdn("~/scripts/plupload/plupload.js"),
                new JsFileWithCdn("~/scripts/plupload/plupload.html4.js"),
                new JsFileWithCdn("~/scripts/plupload/plupload.html5.js"),
                new JsFileWithCdn("~/scripts/plupload/plupload.flash.js"),
                new JsFileWithCdn("~/scripts/jquery.slimscroll.js"),

                new JsFileWithCdn("~/scripts/elastic.js"),
                new JsFileWithCdn("~/scripts/angular-mcustomscrollbar.js"),


                new JsFileWithCdn("~/js/app.js"),
                new JsFileWithCdn("/js/controllers/general/mainCtrl.js"),
                new JsFileWithCdn("/js/controllers/general/shareCtrl.js"),
                new JsFileWithCdn("/js/controllers/search/searchHeaderCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/boxCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/tabCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/qnaCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/uploadCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/settingsCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/dashboardCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/createBoxCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/showFriendsCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/libraryCtrl.js"),
                new JsFileWithCdn("/js/controllers/user/userCtrl.js"),
                new JsFileWithCdn("/js/controllers/item/itemCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/quizCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/quizCreateCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/quizCloseCtrl.js"),
                new JsFileWithCdn("/js/services/dropbox.js"),
                new JsFileWithCdn("/js/services/google.js"),
                new JsFileWithCdn("/js/services/qna.js"),
                new JsFileWithCdn("/js/services/upload.js"),
                new JsFileWithCdn("/js/services/newUpdates.js"),
                new JsFileWithCdn("/js/services/focus.js"),
                new JsFileWithCdn("/js/services/box.js"),
                new JsFileWithCdn("/js/services/item.js"),
                new JsFileWithCdn("/js/services/share.js"),
                new JsFileWithCdn("/js/services/search.js"),
                new JsFileWithCdn("/js/services/quiz.js"),
                new JsFileWithCdn("/js/services/dashboard.js"),
                new JsFileWithCdn("/js/services/user.js"),
                new JsFileWithCdn("/js/services/facebook.js"),
                new JsFileWithCdn("/js/services/userDetails.js"),
                new JsFileWithCdn("/js/services/stacktrace.js"),
                new JsFileWithCdn("/js/directives/ngPlaceholder.js"),
                new JsFileWithCdn("/js/directives/mLoader.js"),
                new JsFileWithCdn("/js/directives/backButton.js"),
                new JsFileWithCdn("/js/directives/focusOn.js"),
                new JsFileWithCdn("/js/directives/selectOnClick.js"),


                new JsFileWithCdn("/js/filters/highlight.js"),
                new JsFileWithCdn("/js/filters/extToClass.js"),
                new JsFileWithCdn("/js/filters/trustedHtml.js"),
                new JsFileWithCdn("/js/filters/actionText.js"),
                new JsFileWithCdn("/js/filters/orderBy.js"),

                new JsFileWithCdn("/js/pubsub.js"),
                new JsFileWithCdn("/scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Scripts/knockout-delegatedEvents.js"),
                new JsFileWithCdn("~/js/Bindings.js"),
                new JsFileWithCdn("/scripts/jquery.slimscroll.js"),
                 new JsFileWithCdn("/js/Utils.js"),
                 new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

                 new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
                new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
                new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                 new JsFileWithCdn("~/js/Logon.js"),

                  new JsFileWithCdn("/js/Cache.js"),
                 new JsFileWithCdn("/js/DataContext.js"),
                 new JsFileWithCdn("~/Js/Statistics.js"),
                 new JsFileWithCdn("/js/Dialog.js"),
                 new JsFileWithCdn("~/js/GmfcnHandler.js"),
                 new JsFileWithCdn("/js/Upload2.js"),
                new JsFileWithCdn("/js/Library.js"),
                new JsFileWithCdn("/js/User.js"),
                new JsFileWithCdn("/js/NotificationsViewModel.js"),
                new JsFileWithCdn("/scripts/CountUp.js"),
                new JsFileWithCdn("/js/ItemViewModel4.js"),
                new JsFileWithCdn("/Scripts/stopwatch.js"),
                new JsFileWithCdn("/js/QuizViewModel.js")
             );

            RegisterJsRegular("angular-shopping",               
                new JsFileWithCdn("~/scripts/uiBootstrapTpls0.11.0.js"),
                new JsFileWithCdn("~/scripts/bindonce.js"),
                new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),
                new JsFileWithCdn("~/js/shopping/app.js"),
                new JsFileWithCdn("~/js/shopping/controllers/mainCtrl.js"),
                new JsFileWithCdn("~/js/shopping/controllers/homeCtrl.js"),
                new JsFileWithCdn("~/js/shopping/controllers/categoryCtrl.js"),
                new JsFileWithCdn("/js/services/stacktrace.js"),
                new JsFileWithCdn("/js/services/userDetails.js"),
                new JsFileWithCdn("/js/services/userDetails.js"),
                new JsFileWithCdn("~/js/shopping/services/store.js"),
                
                new JsFileWithCdn("/js/shopping/directives/productsMenu.js"),

                new JsFileWithCdn("/js/shopping/filters/percentage.js")

                //new JsFileWithCdn("~/js/shopping/controllers/category.js"),
                //new JsFileWithCdn("~/js/shopping/controllers/product.js")
                );

            //RegisterJsRoutes("R_App", "/Js/app.js");
            //RegisterJsRoutes("R_Shim",
            //   // "/Scripts/ng-infinite-scroll.js",
            //    "/Scripts/bindonce.js",
            //    "/Scripts/uiBootstrapTpls0.11.0.js",
            //    "/Scripts/Modernizr.js");

            //RegisterJsRoutes("R_All",

            //    "/js/app.js",
            //    "/js/routes.js",
            //    "/js/services/dependencyResolverFor.js",
            //    "/js/directives/ngPlaceholder.js",
            //    "/js/directives/mLoader.js");

            RegisterJsRoutes("R_OldAll",
                //"/Scripts/knockout-3.0.0.js",
                //"/Scripts/externalScriptLoader.js",
                //"/Scripts/jquery.slimscroll.js",                
                "/js/Utils.js",
                "/js/pubsub.js",
                "/js/DataContext.js",
                // "/Scripts/elasticTextBox.js",
                "/js/Dialog.js",
                "/js/GenericEvents.js",
                "/js/Cache.js");

            //RegisterJsRoutes("R_Box", 
            //    "/js/services/dropbox.js",
            //    "/js/services/google.js",
            //    "/Scripts/draganddrop.js",
            //    "/js/filters/trustedHtml.js",
            //    "/Js/services/qna.js",
            //    "/Js/services/upload.js",
            //    "/Js/controllers/box/boxCtrl.js",
            //    "/Js/controllers/box/tabCtrl.js",
            //    "/Js/controllers/box/qnaCtrl.js",
            //    "/Js/controllers/box/uploadCtrl.js",
            //    "/Js/controllers/box/manageCtrl.js",
            //    "/Js/directives/selectOnClick.js",
            //    "/js/services/facebook.js"
            //    );
            //RegisterJsRoutes("R_DashboardBox",
            //    "/js/services/newUpdates.js",
            //    "/js/services/box.js"
            //    );
            //RegisterJsRoutes("R_BoxItem",
            //    "/Js/services/item.js"
            //    );
            //RegisterJsRoutes("R_BoxQuiz", 
            //    "/Js/services/quiz.js"
            //    );

            //RegisterJsRoutes("R_Dashboard", 
            //    "/Js/services/dashboard.js",

            //    "/js/services/user.js",

            //    "/js/filters/actionText.js",
            //    "/js/filters/orderBy.js",
            //    "/js/controllers/dashboard/createBoxCtrl.js",
            //    "/js/controllers/dashboard/showFriendsCtrl.js",
            //    "/js/controllers/dashboard/dashboardCtrl.js");


            //RegisterJsRoutes("R_Library",             
            //    "/js/controllers/library/libraryCtrl.js",
            //    "/js/Library.js"
            //    );

            //RegisterJsRoutes("R_User",             
            //    "/js/controllers/user/userCtrl.js",
            //    "/js/User.js",
            //    "/scripts/CountUp.js"
            //    );

            //RegisterJsRoutes("R_Item",
            //    "/js/controllers/item/itemCtrl.js",
            //    "/js/ItemViewModel4.js"
            //    );
            // RegisterJsRoutes("R_Quiz",
            //    "/js/controllers/quiz/quizCtrl.js",
            //    "/js/QuizViewModel.js",
            //    "/Scripts/stopwatch.js"
            //);

            RegisterJsRegular("home",
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/Welcome.js"));
            RegisterJsRegular("homeMobile",
                new JsFileWithCdn("~/Js/Mobile/Logon.js"),
                new JsFileWithCdn("~/Js/Mobile/Welcome.js"));

            RegisterJsRegular("ChooseLib",
                //new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
                new JsFileWithCdn("~/Js/Cache2.js"),
                new JsFileWithCdn("~/Js/DataContext2.js"),
                new JsFileWithCdn("~/Js/GenericEvents2.js"),
                new JsFileWithCdn("~/Js/LibraryChoose.js"));

            RegisterJsRegular("MChooseLib",
                new JsFileWithCdn("~/Js/Mobile/MLibraryChoose.js"));


            RegisterJsRegular("General",
                new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.js"),// the script is too small
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                new JsFileWithCdn("~/Scripts/Modernizr.js"),

                //new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                new JsFileWithCdn("~/Js/Utils2.js"),
                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),

                new JsFileWithCdn("~/Js/pubsub2.js"),

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
                //new JsFileWithCdn("~/Scripts/jquery.mousewheel.min.js"),

                //new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.js"),

                //new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.concat.min.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.html4.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.html5.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.flash.js"),
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/Cache2.js"),
                new JsFileWithCdn("~/Js/DataContext2.js"),
                new JsFileWithCdn("~/Js/GenericEvents2.js"),
                new JsFileWithCdn("~/Js/Navigation.js"),


                new JsFileWithCdn("~/Js/Dialog2.js"), //dialog message
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
                //new JsFileWithCdn("~/Js/Library.js"),


                //box Page
                //new JsFileWithCdn("~/Js/BoxViewModel2.js"),
                //new JsFileWithCdn("~/Js/BoxItemsViewModel2.js"),
                //new JsFileWithCdn("~/Js/BoxSettings.js"),
                //new JsFileWithCdn("~/Js/Upload2.js"),
                new JsFileWithCdn("~/Js/Invite.js"),
                //new JsFileWithCdn("/Js/Share.js"),
                //new JsFileWithCdn("~/Js/QnA.js"),

                //account settings
                new JsFileWithCdn("~/Js/AccountSettings.js"),

                //item
                //new JsFileWithCdn("~/Js/ItemViewModel4.js"),

                //signalR
                new JsFileWithCdn("~/Js/RT.js"),

                //Social
                new JsFileWithCdn("~/Js/SocialConnect.js"),

                //User Page
                //new JsFileWithCdn("~/Scripts/CountUp.js"),
                //new JsFileWithCdn("~/Js/User.js"),

                //Search page
                new JsFileWithCdn("~/Js/SearchDropdown.js"),
                new JsFileWithCdn("~/Js/Search.js")

                //Quiz
                //new JsFileWithCdn("~/Js/QuizCreate.js"),
                //new JsFileWithCdn("~/Js/QuizViewModel.js"),
                //new JsFileWithCdn("~/Scripts/stopwatch.js")
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
                                    new JsFileWithCdn("~/Js/Utils2.js"),
                                    new JsFileWithCdn("~/Js/pubsub2.js"),
                                    new JsFileWithCdn("~/Js/Cache2.js"),
                                    new JsFileWithCdn("~/Js/DataContext2.js"),
                                    new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js"));
            RegisterJsRegular("mobile",
                  new JsFileWithCdn("~/Scripts/jquery-2.1.0.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.0/jquery.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jquery.validate/1.11.1/jquery.validate.min.js"),
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
            CopyFilesToCdn("/gzip/", "*.*", SearchOption.TopDirectoryOnly);
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
            //jsBundle.ForceRelease();
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
                CopyFilesToCdn("~/gzip/", "*.js", SearchOption.TopDirectoryOnly);
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

            var directoryPath = server.MapPath(directoryRelativePath);
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var filesPath = Directory.GetFiles(directoryPath, fileSearchOption, options);

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