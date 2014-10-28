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
        private static readonly Dictionary<string, string> CssBundles = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> JsBundles = new Dictionary<string, string>();
      
        private static readonly string CdnLocation = GetValueFromCloudConfig();

        public static string CssLink(string key)
        {
            string retVal;
            CssBundles.TryGetValue(key, out retVal);

            return retVal;
        }
        public static string JsLink(string key)
        {
            return JsBundles[key];
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
                "~/Content/StoreRtl.css",
                "~/Content/SetupSchoolRtl.css",
                "~/Content/HeaderFooterRtl.css",
                "~/Content/BoxFeedRtl.css",
                "~/Content/ItemRtl.css");

            RegisterCss("newcore3", "~/Content/General.css",
                "~/Content/HeaderFooter.css",
                "~/Content/Site3.css",
                "~/Content/AccountInfo.css",
                "~/Content/Animations.css",
                "~/Content/UserPage.css",
                "~/Content/Search.css",
                "~/Content/Sidebar.css",
                "~/Content/SetupSchool.css",
                "~/Content/Modal.css",
                //"~/Content/QnA.css",
                "~/Content/BoxFeed.css",
                "~/Content/Quiz.css",
                "~/Content/Invite.css",
                "~/Content/Upload.css",
                "~/Content/Box3.css",
                "~/Content/Item.css",
                "~/Content/Settings.css",
                "~/Content/DashLib.css",
                "~/Content/Store.css",
                "~/Content/jquery.mCustomScrollbar.css");

            RegisterCss("staticRtl", "~/Content/GeneralRtl.css",
                "~/Content/StaticRtl.css",
                "~/Content/HeaderFooterRtl.css");

            RegisterCss("static", "~/Content/General.css",
                "~/Content/HeaderFooter.css",
                "~/Content/Animations.css",
                "~/Content/Static.css");


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
                                new JsFileWithCdn("~/scripts/jquery-2.1.1.min.js",
                    "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                    new JsFileWithCdn("~/scripts/underscore.js"),
                new JsFileWithCdn("~/scripts/angular.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.24/angular.min.js"),


                new JsFileWithCdn("~/scripts/angular-route.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.24/angular-route.min.js"),
                new JsFileWithCdn("~/js/services/cookies.js"),

            new JsFileWithCdn("~/scripts/angular-sanitize.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.24/angular-sanitize.min.js"),

                    new JsFileWithCdn("~/scripts/angular-animate.js",
                    "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.24/angular-animate.min.js"),
                    new JsFileWithCdn("~/scripts/angulartics.js"),
                        new JsFileWithCdn("~/scripts/angulartics-ga.js"),
                        new JsFileWithCdn("/scripts/angular-appinsights.js"),
                        new JsFileWithCdn("/scripts/angular-cache-2.4.1.js"),                        
                new JsFileWithCdn("~/scripts/stacktrace.js"),
                new JsFileWithCdn("~/scripts/underscore.js")

                                    );

            RegisterJsRegular("angular-layout3",

                new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),
                new JsFileWithCdn("~/scripts/bindonce.js"),                
                new JsFileWithCdn("~/scripts/uiBootstrapTpls0.11.0.js"),
                new JsFileWithCdn("~/scripts/angular-draganddrop.js"),
                new JsFileWithCdn("~/scripts/angular-debounce.js"),
                new JsFileWithCdn("~/scripts/jquery.mousewheel.min.js"),
                new JsFileWithCdn("~/scripts/jquery.mCustomScrollbar.js"),
                new JsFileWithCdn("~/scripts/Modernizr.js"),

                new JsFileWithCdn("~/scripts/plupload2/moxie.js"),
                new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),                
                new JsFileWithCdn("~/scripts/plupload2/angular-plupload.js"),

                new JsFileWithCdn("~/scripts/elastic.js"),
                new JsFileWithCdn("~/scripts/angular-mcustomscrollbar.js"),
                new JsFileWithCdn("~/scripts/svg4everybody.js"),

                new JsFileWithCdn("/js/modules/displayTime.js"),                
                
                new JsFileWithCdn("/js/modules/angular-timer.js"),
                new JsFileWithCdn("/js/modules/wizard.js"),
                new JsFileWithCdn("~/js/modules/textDirection.js"),

                new JsFileWithCdn("~/js/app.js"),

                new JsFileWithCdn("/js/controllers/general/mainCtrl.js"),
                new JsFileWithCdn("/js/controllers/general/shareCtrl.js"),
                new JsFileWithCdn("/js/controllers/general/uploadListCtrl.js"),
                new JsFileWithCdn("/js/controllers/account/settingsCtrl.js"),
                new JsFileWithCdn("/js/controllers/account/notificationsCtrl.js"),                
                new JsFileWithCdn("/js/controllers/account/notificationSettingsCtrl.js"),                
                new JsFileWithCdn("/js/controllers/search/searchCtrl.js"),
                new JsFileWithCdn("/js/controllers/search/searchHeaderCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/boxCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/boxTabsCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/sideBarCtrl.js"),                
                new JsFileWithCdn("/js/controllers/box/boxItemsCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/boxInviteCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/uploadPopupCtrl.js"),                
                new JsFileWithCdn("/js/controllers/box/boxQuizzesCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/boxMembersCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/tabCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/qnaCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/uploadCtrl.js"),
                new JsFileWithCdn("/js/controllers/box/settingsCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/dashboardCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/createBoxWizardCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/createBoxCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/inviteEmailCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/createAcademicBoxCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/SocialInviteCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/showFriendsCtrl.js"),
                new JsFileWithCdn("/js/controllers/dashboard/inviteCloudentsCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/libraryCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/libChooseCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/createBoxLibCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/createDepartmentCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/restrictionPopUpCtrl.js"),
                new JsFileWithCdn("/js/controllers/library/libraryRenameCtrl.js"),
                new JsFileWithCdn("/js/controllers/user/userCtrl.js"),
                new JsFileWithCdn("/js/controllers/item/itemCtrl.js"),
                new JsFileWithCdn("/js/controllers/item/itemFullScreenCtrl.js"),
                new JsFileWithCdn("/js/controllers/item/itemRenameCtrl.js"),
                new JsFileWithCdn("/js/controllers/item/itemFlagCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/quizCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/challengeCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/quizCreateCtrl.js"),
                new JsFileWithCdn("/js/controllers/quiz/quizCloseCtrl.js"),
                new JsFileWithCdn("/js/services/ajaxService.js"),
                new JsFileWithCdn("/js/services/dropbox.js"),
                new JsFileWithCdn("/js/services/google.js"),
                new JsFileWithCdn("/js/services/qna.js"),
                new JsFileWithCdn("/js/services/upload.js"),
                new JsFileWithCdn("/js/services/newUpdates.js"),
                new JsFileWithCdn("/js/services/focus.js"),
                new JsFileWithCdn("/js/services/box.js"),
                new JsFileWithCdn("/js/services/item.js"),
                new JsFileWithCdn("/js/services/library.js"),
                new JsFileWithCdn("/js/services/account.js"),
                new JsFileWithCdn("/js/services/share.js"),
                new JsFileWithCdn("/js/services/library.js"),
                new JsFileWithCdn("/js/services/search.js"),
                new JsFileWithCdn("/js/services/quiz.js"),
                new JsFileWithCdn("/js/services/dashboard.js"),
                new JsFileWithCdn("/js/services/user.js"),
                new JsFileWithCdn("/js/services/facebook.js"),
                new JsFileWithCdn("/js/services/userDetails.js"),
                new JsFileWithCdn("/js/services/stacktrace.js"),
                new JsFileWithCdn("/js/directives/ngPlaceholder.js"),
                new JsFileWithCdn("/js/directives/storageSpace.js"),
                new JsFileWithCdn("/js/directives/dropZone.js"),                
                new JsFileWithCdn("/js/directives/fbBlock.js"),
                new JsFileWithCdn("/js/directives/loadSpinner.js"),

                new JsFileWithCdn("/js/directives/scrollToTop.js"),
                new JsFileWithCdn("/js/directives/focusForm.js"),
                //new JsFileWithCdn("/js/directives/lazySrc.js"),



                new JsFileWithCdn("/js/directives/mLoader.js"),
                new JsFileWithCdn("/js/directives/backButton.js"),
                new JsFileWithCdn("/js/directives/quizGraph.js"),
                new JsFileWithCdn("/js/directives/focusOn.js"),
                new JsFileWithCdn("~/js/directives/countTo.js"),
                new JsFileWithCdn("~/js/directives/userTooltip.js"),
                new JsFileWithCdn("~/js/directives/userDetails.js"),
                new JsFileWithCdn("~/js/directives/departmentsTooltip.js"),
                
                new JsFileWithCdn("/js/directives/facebookFeed.js"),
                new JsFileWithCdn("/js/directives/selectOnClick.js"),
                new JsFileWithCdn("/js/directives/facebookFeed.js"),
                new JsFileWithCdn("/js/directives/contentEditable.js"),
                new JsFileWithCdn("/js/directives/rateStar.js"),


                new JsFileWithCdn("/js/filters/highlight.js"),
                new JsFileWithCdn("/js/filters/fileSize.js"),
                new JsFileWithCdn("/js/filters/extToClass.js"),
                new JsFileWithCdn("/js/filters/trustedHtml.js"),
                new JsFileWithCdn("/js/filters/actionText.js"),
                new JsFileWithCdn("/js/filters/orderBy.js"),
                new JsFileWithCdn("/js/filters/stringFormat.js"),

                new JsFileWithCdn("/js/pubsub.js"),
                //new JsFileWithCdn("/scripts/knockout-3.0.0.js"),
                //new JsFileWithCdn("~/scripts/knockout-delegatedEvents.js"),
                //new JsFileWithCdn("~/js/Bindings.js"),
                new JsFileWithCdn("/scripts/jquery.slimscroll.js"),
                 new JsFileWithCdn("/js/Utils.js"),
                 new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

                 new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
                new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
                new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                 new JsFileWithCdn("~/js/Logon.js"),

                  new JsFileWithCdn("/js/Cache.js"),
                 new JsFileWithCdn("/js/DataContext.js"),
                 new JsFileWithCdn("/js/Dialog.js"),
                 new JsFileWithCdn("~/js/GmfcnHandler.js"),
                 //new JsFileWithCdn("/js/Upload2.js"),
                //new JsFileWithCdn("/js/Library.js"),
                //new JsFileWithCdn("/js/User.js"),
                new JsFileWithCdn("/js/NotificationsViewModel.js"),
                new JsFileWithCdn("/scripts/CountUp.js")
                //new JsFileWithCdn("/scripts/stopwatch.js")
             );

            RegisterJsRegular("angular-store",
                //new JsFileWithCdn("/js/controllers/store/homeCtrl.js"),
                new JsFileWithCdn("/js/controllers/store/productCtrl.js"),
                new JsFileWithCdn("/js/controllers/store/contactCtrl.js"),
                new JsFileWithCdn("/js/controllers/store/viewCtrl.js"),
                new JsFileWithCdn("/js/controllers/store/checkoutCtrl.js"),
                new JsFileWithCdn("/js/controllers/store/categoryCtrl.js"),
                new JsFileWithCdn("/js/controllers/store/carouselCtrl.js"),
                new JsFileWithCdn("/js/services/store.js"),

                new JsFileWithCdn("/js/directives/store/productsMenu.js"),
                new JsFileWithCdn("/js/directives/dataBag.js"),
                new JsFileWithCdn("/js/directives/store/categoryLink.js"),

                new JsFileWithCdn("/js/filters/store/percentage.js")
                );


            RegisterJsRegular("home",
                new JsFileWithCdn("~/Js/Logon.js"),
                new JsFileWithCdn("~/Js/Welcome.js"));
            RegisterJsRegular("homeMobile",
                new JsFileWithCdn("~/Js/Mobile/Logon.js"),
                new JsFileWithCdn("~/Js/Mobile/Welcome.js"));

            RegisterJsRegular("MChooseLib",
                new JsFileWithCdn("~/Js/Mobile/MLibraryChoose.js"));


            RegisterJsRegular("General",
                new JsFileWithCdn("~/scripts/jquery-2.1.1.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
                new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
                new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                new JsFileWithCdn("~/scripts/Modernizr.js"),

                //new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                new JsFileWithCdn("~/Js/Utils2.js"),
                new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

                new JsFileWithCdn("~/Js/pubsub2.js"),

                new JsFileWithCdn("~/Js/GmfcnHandler.js")
                //new JsFileWithCdn("~/Js/externalScriptsInitializer.js")
                );
            RegisterJsRegular("faq", new JsFileWithCdn("~/Js/externalScriptsInitializer.js"));


            #region layout3
            //RegisterJsRegular("cd1",
            //    new JsFileWithCdn("~/Scripts/jquery-ui-1.10.4.min.js"),
            //    new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
            //    new JsFileWithCdn("~/Scripts/knockout-delegatedEvents.js"),
            //    new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
            //    new JsFileWithCdn("~/Scripts/jquery.slimscroll.js"),

            //    new JsFileWithCdn("~/Scripts/elasticTextBox.js"),
            //    //new JsFileWithCdn("~/Scripts/jquery.mousewheel.min.js"),

            //    //new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.js"),

            //    //new JsFileWithCdn("~/Scripts/jquery.mCustomScrollbar.concat.min.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.html4.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.html5.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.flash.js"),
            //    new JsFileWithCdn("~/Js/Logon.js"),
            //    new JsFileWithCdn("~/Js/Cache2.js"),
            //    new JsFileWithCdn("~/Js/DataContext2.js"),
            //    new JsFileWithCdn("~/Js/GenericEvents2.js"),


            //    new JsFileWithCdn("~/Js/Dialog2.js"), //dialog message
            //    new JsFileWithCdn("~/Js/Autocomplete.js"), //dialog message
            //    new JsFileWithCdn("~/Js/Tooltip.js"), //dialog message


            //   new JsFileWithCdn("~/Js/TooltipGuide.js"),


            //    //header
            //    new JsFileWithCdn("~/Js/NotificationsViewModel.js"),

             
            //    new JsFileWithCdn("~/Js/Invite.js"),
             

            //    //account settings
            //    new JsFileWithCdn("~/Js/AccountSettings.js"),

            //    //item
            //    //new JsFileWithCdn("~/Js/ItemViewModel4.js"),

            //    //signalR
            //   // new JsFileWithCdn("~/Js/RT.js"),

            //    //Social
            //    new JsFileWithCdn("~/Js/SocialConnect.js"),

            //    //Search page
            //    new JsFileWithCdn("~/Js/SearchDropdown.js"),
            //    new JsFileWithCdn("~/Js/Search.js")
             
            //    );

            #endregion layout3


            #region mobile
            RegisterJsRegular("mobileItem", new JsFileWithCdn("~/Scripts/jquery-2.1.1.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                                    new JsFileWithCdn("~/Js/Utils2.js"),
                                    new JsFileWithCdn("~/Js/pubsub2.js"),
                                    new JsFileWithCdn("~/Js/Cache2.js"),
                                    new JsFileWithCdn("~/Js/DataContext2.js"),
                                    new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js"));
            RegisterJsRegular("mobile",
                  new JsFileWithCdn("~/Scripts/jquery-2.1.1.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
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
               // new JsFileWithCdn("~/Js/Mobile/MCommentsViewModel.js"),

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
                CssBundles.Add(key, cssbundle.Render("~/gzip/c#.css"));
                CopyFilesToCdn("~/gzip/", "*.css", SearchOption.TopDirectoryOnly);
            }
            else
            {
                CssBundles.Add(key, cssbundle.Render("~/cdn/gzip/c#.css"));
            }


        }

        private static string RegisterJs(IEnumerable<JsFileWithCdn> jsFiles, JavaScriptBundle javaScriptBundleImp)
        {
            var jsBundle = javaScriptBundleImp;
            //jsBundle.ForceDebug();
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
                CopyFilesToCdn("~/gzip/", "*.js", SearchOption.TopDirectoryOnly);
                return jsBundle.Render("~/gzip/j#.js");
            }
            return jsBundle.Render("~/cdn/gzip/j#.js");
        }

        private static void RegisterJsRegular(string key, params JsFileWithCdn[] jsFiles)
        {
            JsBundles.Add(key, RegisterJs(jsFiles, SquishIt.Framework.Bundle.JavaScript()));
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