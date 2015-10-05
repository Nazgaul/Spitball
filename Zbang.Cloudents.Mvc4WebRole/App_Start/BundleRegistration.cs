using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class BundleRegistration
    {
        public static void RegisterBundles()
        {
            BundleConfig.RegisterBundle(RegisterCss(), RegisterJs());

        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<string>>> RegisterCss()
        {
            var cssDictionary = new Dictionary<string, IEnumerable<string>>
            {
                {"lang.ru-RU", new[] {"~/Content/lang.ru-RU.css"}},
                {"lang.he-IL", new[] {"~/Content/lang.he-IL.css"}},
                {"lang.ar-AE", new[] {"~/Content/lang.ar-AE.css"}},

                //{
                //    "newcore3", new[]
                //    {
                //        "~/Content/Normalize.css",
                //        "~/Content/General.css",
                //        "~/Content/Header.css",
                //        "~/Content/Site3.css",
                //        "~/Content/SVG.css",
                //        "~/Content/Animations.css",
                //        "~/Content/UserPage.css",
                //        "~/Content/Search.css",
                //        "~/Content/Sidebar.css",
                //        "~/Content/SetupSchool.css",
                //        "~/Content/Modal.css",
                //        "~/Content/BoxFeed.css",
                //        "~/Content/Quiz.css",
                //        "~/Content/Invite.css",
                //        "~/Content/Upload.css",
                //        "~/Content/Box3.css",
                //        "~/Content/Item.css",
                //        "~/Content/Settings.css",
                //        "~/Content/DashLib.css",
                //        "~/Content/Home.css",
                //        "~/Content/jquery.mCustomScrollbar.css",
                //        "~/Content/Letter.css",
                //        "~/Content/RtlFix.css",
                //        "~/Content/textAngular.css"
                       
                //    }
                //},

                {
                    "static", new[]
                    {
                        "~/Content/Normalize.css",
                        "~/Content/General.css",
                        "~/Content/Header.css",
                        "~/Content/Animations.css",
                        "~/Content/Static.css"
                    }
                },
                //{
                //    "mobile", new []
                //    {
                //        "~/Content/Mobile.css"
                //    }
                //},
                {
                    "homePage", new[]
                    {
                         "~/Content/HomePage/extras.css",
                         "~/Content/HomePage/bootstrap.css",
                         "~/Content/HomePage/theme.css",
                         //"~/Content/HomePage/restaurant.css",
                         "~/Content/HomePage/landing.css"

                    }
                },
                  {
                    "signin", new[]
                    {
                        "~/Content/signin/font-awesome.css",
                        "~/Content/signin/simple-line-icons.css",
                        "~/Content/homepage/bootstrap.css",
                        "~/Content/signin/uniform.default.css",
                        "~/Content/signin/select2.css",
                        "~/Content/signin/login-soft.css",
                        "~/Content/signin/components-md.css",
                        "~/Content/signin/plugins-md.css",
                        "~/Content/site/layout.css",
                        "~/Content/signin/default.css",
                        "~/Content/signin/custom.css",
                        "~/Content/signin/customRtl.css"
                    }
                },
                 {
                    "site4", new[]
                    {
                        //"~/bower_components/angular-material/angular-material.css",
                        "~/Content/signin/font-awesome.css",
                        "~/Content/signin/simple-line-icons.css",
                        "~/Content/homepage/bootstrap.css",
                        "~/Content/signin/uniform.default.css",
                        "~/Content/site/bootstrap-switch.css",
                        "~/Content/site/portfolio.css",
                        "~/content/site/profile.css",
                        "~/Content/site/components-rounded.css",
                        "~/Content/site/plugins.css",
                        "~/Content/site/layout.css",
                        "~/Content/site/light.css",
                        "~/Content/site/master.css",
                        "~/Content/site/menu.css",

                        "~/content/site/morphsearch.css",
                        
                        "~/content/site/general.css",
                        "~/Content/site/dashboard.css",
                        "~/Content/site/box.css",
                        //"~/Content/site/boxMembers.css",
                        "~/Content/site/user.css",
                        "~/Content/site/accountSettings.css",
                        "~/Content/site/library.css",
                        "~/Content/site/item.css",
                        "~/Content/site/quiz.css",
                        //"~/Content/signin/select2.css",
                        //"~/Content/signin/login-soft.css",
                        //"~/Content/signin/components-md.css",
                        //"~/Content/signin/plugins-md.css",
                        //"~/Content/signin/layout.css",
                        //"~/Content/signin/default.css",
                        //"~/Content/signin/custom.css",
                        //"~/Content/signin/customRtl.css"

                        "~/Content/site/rtlFix.css"
                    }
                },
            };
            return cssDictionary;
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>
            {
                //{
                //    "angular", new[]
                //    {
                //        new JsFileWithCdn("~/scripts/jquery.min.js",
                //            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                //        new JsFileWithCdn("~/scripts/underscore.js"),
                //        new JsFileWithCdn("~/scripts/angular.min.js",
                //            "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular.min.js"),
                //        new JsFileWithCdn("~/scripts/angular-route.js"
                //            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-route.min.js"
                //            ),
                //        new JsFileWithCdn("~/scripts/angular-cookies.js"
                //            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-cookies.min.js"
                //            ),
                //            new JsFileWithCdn("~/scripts/angular-messages.js"
                //            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-messages.min.js"
                //            ),
                //        //new JsFileWithCdn("~/scripts/angular-sanitize.js"
                //        //    //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-sanitize.min.js"
                //        //    ),
                //        new JsFileWithCdn("~/scripts/angular-animate.js",
                //            "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-animate.min.js"),
                //            new JsFileWithCdn("~/scripts/textAngular/textAngular-rangy.min.js"),
                //            new JsFileWithCdn("~/scripts/textAngular/textAngular-sanitize.js"),
                //            new JsFileWithCdn("~/scripts/textAngular/textAngularSetup.js"),
                //            new JsFileWithCdn("~/scripts/textAngular/textAngular.js"),


                //        new JsFileWithCdn("~/scripts/angulartics.js"), new JsFileWithCdn("~/scripts/angulartics-ga.js"),
                //        //new JsFileWithCdn("~/scripts/angular-appinsights.js"),
                //        new JsFileWithCdn("~/scripts/angular-cache-2.4.1.js"),
                //        new JsFileWithCdn("~/scripts/stacktrace.js"),
                //        new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),
                //        //new JsFileWithCdn("~/scripts/bindonce.js"),
                //        new JsFileWithCdn("~/scripts/ui-bootstrap-custom-tpls-0.13.0.js"),
                //        new JsFileWithCdn("~/scripts/angular-draganddrop.js"),
                //        new JsFileWithCdn("~/scripts/angular-debounce.js"),
                //        new JsFileWithCdn("~/scripts/jquery.mousewheel.min.js"),
                //        new JsFileWithCdn("~/scripts/jquery.mCustomScrollbar.js"),
                //        new JsFileWithCdn("~/scripts/Modernizr.js"),

                //        new JsFileWithCdn("~/scripts/plupload2/moxie.js"),
                //        new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),
                //        new JsFileWithCdn("~/scripts/plupload2/angular-plupload.js"),
                //        new JsFileWithCdn("~/scripts/CountUp.js"),
                //        new JsFileWithCdn("~/scripts/elastic.js"),
                //        new JsFileWithCdn("~/scripts/angular-mcustomscrollbar.js"),
                //        new JsFileWithCdn("~/scripts/svg4everybody.js"),

                //        new JsFileWithCdn("~/js/modules/displayTime.js"),

                //        new JsFileWithCdn("~/js/modules/angular-timer.js"),
                //        //new JsFileWithCdn("/js/modules/lazySrc.js"),                        
                //        new JsFileWithCdn("~/js/modules/wizard.js"),
                //        new JsFileWithCdn("~/js/modules/textDirection.js"),

                //        new JsFileWithCdn("~/js/app.js"),

                //        new JsFileWithCdn("/js/controllers/general/mainCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/general/shareCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/general/uploadListCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/settingsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/loginWrapperCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/accountCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/notificationsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/congratsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/notificationSettingsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/userDetailsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/loginCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/search/searchCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/boxCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/sideBarCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/leavePromptCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/boxTabsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/boxItemsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/boxInviteCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/uploadPopupCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/boxQuizzesCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/boxMembersCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/tabCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/qnaCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/uploadCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/box/settingsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/dashboardCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/createBoxWizardCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/createBoxCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/inviteEmailCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/createAcademicBoxCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/SocialInviteCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/dashboard/inviteCloudentsCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/library/libraryCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/library/libChooseCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/library/createDepartmentCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/library/restrictionPopUpCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/library/libraryRenameCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/user/userCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/item/itemCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/item/itemRegCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/item/itemFullScreenCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/item/itemRenameCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/item/itemFlagCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/quiz/quizCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/quiz/challengeCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/quiz/quizCreateCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/quiz/quizCloseCtrl.js"),
                //        new JsFileWithCdn("/js/controllers/account/SpitballCtrl.js"),
                //        new JsFileWithCdn("/js/services/ajaxService.js"),
                //        new JsFileWithCdn("/js/services/modalWrapper.js"),
                //        new JsFileWithCdn("/js/services/versionChecker.js"),
                //        new JsFileWithCdn("/js/services/login.js"),
                //        new JsFileWithCdn("/js/services/dropbox.js"),
                //        new JsFileWithCdn("/js/services/google.js"),
                //        new JsFileWithCdn("/js/services/qna.js"),
                //        new JsFileWithCdn("/js/services/notificationManager.js"),
                //        new JsFileWithCdn("/js/services/upload.js"),
                //        new JsFileWithCdn("/js/services/newUpdates.js"),
                //        new JsFileWithCdn("/js/services/focus.js"),
                //        new JsFileWithCdn("/js/services/box.js"),
                //        new JsFileWithCdn("/js/services/item.js"),
                //        new JsFileWithCdn("/js/services/library.js"),
                //        new JsFileWithCdn("~/js/services/userPoints.js"),
                //        new JsFileWithCdn("/js/services/account.js"),
                //        new JsFileWithCdn("/js/services/share.js"),
                //        new JsFileWithCdn("/js/directives/boxLogo.js"),
                //        //new JsFileWithCdn("/js/services/htmlCache.js"),                        
                //        new JsFileWithCdn("/js/services/resourceManager.js"),
                //        new JsFileWithCdn("/js/services/notificationHandler.js"),
                //        new JsFileWithCdn("/js/services/library.js"),
                //        new JsFileWithCdn("/js/services/search.js"),
                //        new JsFileWithCdn("/js/services/quiz.js"),
                //        new JsFileWithCdn("/js/services/dashboard.js"),
                //        new JsFileWithCdn("/js/services/user.js"),
                //        new JsFileWithCdn("/js/services/facebook.js"),
                //        new JsFileWithCdn("/js/services/userDetails.js"),   
                //        //new JsFileWithCdn("/js/services/storeHandler.js"),   
                //        new JsFileWithCdn("/js/services/stacktrace.js"),
                //        new JsFileWithCdn("/js/directives/ngMatch.js"),
                //        new JsFileWithCdn("/js/directives/searchCoords.js"),
                //        new JsFileWithCdn("/js/directives/userimage.js"),
                //        new JsFileWithCdn("/js/directives/fileReader.js"),
                //        new JsFileWithCdn("/js/directives/ngPlaceholder.js"),
                //        new JsFileWithCdn("/js/directives/storageSpace.js"),
                //        new JsFileWithCdn("/js/directives/dropZone.js"),
                //        new JsFileWithCdn("/js/directives/fbBlock.js"),
                //        new JsFileWithCdn("/js/directives/letter.js"),
                //        new JsFileWithCdn("/js/directives/loadSpinner.js"),
                //        new JsFileWithCdn("~/js/directives/userCoupon.js"),
                //        new JsFileWithCdn("/js/directives/scrollToTop.js"),
                //        new JsFileWithCdn("/js/directives/focusForm.js"),


                //        new JsFileWithCdn("/js/directives/homePageScroll.js"),
                //        new JsFileWithCdn("/js/directives/itemFullscreen.js"),
                //        new JsFileWithCdn("/js/directives/itemScroll.js"),
                //        new JsFileWithCdn("/js/directives/mLoader.js"),
                //        new JsFileWithCdn("/js/directives/login.js"),
                //        new JsFileWithCdn("/js/directives/quizGraph.js"),
                //        new JsFileWithCdn("/js/directives/homeVideo.js"),
                //        new JsFileWithCdn("/js/directives/focusOn.js"),
                //        new JsFileWithCdn("~/js/directives/countTo.js"),
                //        new JsFileWithCdn("~/js/directives/userPoints.js"),
                //        new JsFileWithCdn("~/js/directives/userTooltip.js"),
                //        new JsFileWithCdn("~/js/directives/userDetails.js"),
                //        new JsFileWithCdn("~/js/directives/departmentsTooltip.js"),
                //        new JsFileWithCdn("/js/directives/moxieDropzone.js"),
                //        new JsFileWithCdn("/js/directives/facebookFeed.js"),
                //        new JsFileWithCdn("/js/directives/selectOnClick.js"),
                //        new JsFileWithCdn("/js/directives/facebookFeed.js"),
                //        new JsFileWithCdn("/js/directives/contentEditable.js"),
                //        new JsFileWithCdn("/js/directives/rateStar.js"),
                //        new JsFileWithCdn("/js/directives/boxItemTooltip.js"),
                //        new JsFileWithCdn("/js/filters/escapeHtmlChars.js"),
                //        new JsFileWithCdn("/js/filters/kNumber.js"),
                //        new JsFileWithCdn("/js/filters/defaultImage.js"),
                //        new JsFileWithCdn("/js/filters/fileSize.js"),
                //        new JsFileWithCdn("/js/filters/extToClass.js"),
                //        new JsFileWithCdn("/js/filters/actionText.js"),
                //        new JsFileWithCdn("/js/filters/highlight.js"),
                //        new JsFileWithCdn("/js/filters/orderBy.js"),
                //        new JsFileWithCdn("/js/filters/stringFormat.js"),
                //        new JsFileWithCdn("/js/directives/misc.js"),
                //        new JsFileWithCdn("/js/filters/store/percentage.js")
                //    }
                //},
                {
                    "static", new[]
                    {
                        new JsFileWithCdn("~/Scripts/validatinator.min.js"),
                         new JsFileWithCdn("~/scripts/svg4everybody.js"),
                        new JsFileWithCdn("~/Js/staticShim.js"),
                        new JsFileWithCdn("~/Js/Logon.js"),
                        new JsFileWithCdn("~/Js/HomePage.js")
                    }
                },
                //{
                //    "General", new[]
                //    {
                //        new JsFileWithCdn("~/scripts/jquery.min.js",
                //            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js")
                //        //new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
                //        //new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
                //        //new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                //        //new JsFileWithCdn("~/scripts/Modernizr.js"),

                //        ////new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                //        //new JsFileWithCdn("~/Js/Utils2.js"),
                //        //new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

                //        //new JsFileWithCdn("~/Js/pubsub2.js"),
                //        //new JsFileWithCdn("~/scripts/svg4everybody.js")
                //    }
                //},
                //{
                //    "faq", new[]
                //    {
                //        new JsFileWithCdn("~/scripts/jquery.min.js",
                //            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js")
                //    }
                //},
                {
                    "homePage", new[]
                    {
                        new JsFileWithCdn("~/js/homePage/theme.js"),
                        //new JsFileWithCdn("~/Scripts/browser-deeplink.min.js")
                        new JsFileWithCdn("~/js/homePage/homeScreen.js")

                    }
                },
                {
                    "signin", new[]
                    {


                        new JsFileWithCdn("~/js/signin/jquery.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery-migrate.min.js"),
                        new JsFileWithCdn("~/js/signin/bootstrap.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.blockui.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.uniform.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.cokie.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.validate.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.backstretch.min.js"),
                        new JsFileWithCdn("~/js/signin/select2.min.js"),
                        new JsFileWithCdn("~/js/signin/demo.js"),
                        new JsFileWithCdn("~/js/signin/metronic.js"),
                        new JsFileWithCdn("~/js/signin/layout.js"),
                        new JsFileWithCdn("~/js/signin/login-soft.js")
                    }
                },
                {
                    "site4", new[]
                    {

                        new JsFileWithCdn("~/js/signin/jquery.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery-migrate.min.js"),
                        
                        //new JsFileWithCdn("~/scripts/stacktrace.js"), 

                        //new JsFileWithCdn("~/scripts/site/jquery-ui.min.js"), //?
                        new JsFileWithCdn("~/js/signin/bootstrap.min.js"),
                        new JsFileWithCdn("~/scripts/site/bootstrap-hover-dropdown.js"),
                        new JsFileWithCdn("~/scripts/site/jquery.slimscroll.js"),
                        new JsFileWithCdn("~/js/signin/jquery.blockui.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.cokie.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery.uniform.min.js"),
                        //new JsFileWithCdn("~/scripts/site/bootstrap-switch.js"),//?


                        new JsFileWithCdn("~/scripts/angular.min.js",
                            "https://ajax.googleapis.com/ajax/libs/angularjs/1.4.4/angular.min.js"),
                        new JsFileWithCdn("~/scripts/angular-sanitize.js"
                            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.4.4/angular-sanitize.min.js"
                            ),
                        new JsFileWithCdn("~/scripts/angular-ui-router.js"),
                        new JsFileWithCdn("~/scripts/angulartics.js"),
                        new JsFileWithCdn("~/scripts/angulartics-ga.js"),
                        new JsFileWithCdn("~/scripts/angular-cache-2.4.1.js"),
                        //new JsFileWithCdn("~/bower_components/angular-animate/angular-animate.js"),
                        //new JsFileWithCdn("~/bower_components/angular-aria/angular-aria.js"),
                        //new JsFileWithCdn("~/bower_components/angular-material/angular-material.js"),
                        new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),

                        new JsFileWithCdn("~/scripts/plupload2/moxie.js"),
                        new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),
                        new JsFileWithCdn("~/scripts/plupload2/angular-plupload2.js"),

                        new JsFileWithCdn("~/scripts/ui-bootstrap-custom-tpls-0.13.4.min.js"),
                        new JsFileWithCdn("~/scripts/site/bootstrap-tabdrop.js"),
                        new JsFileWithCdn("~/scripts/site/jquery.mixitup.min.js"),
                        new JsFileWithCdn("~/scripts/site/jquery.dataTables.min.js"),

                        new JsFileWithCdn("~/js/signin/metronic.js"),
                        new JsFileWithCdn("~/js/signin/layout.js"),
                        //new JsFileWithCdn("~/scripts/site/quick-sidebar.js"),
                        new JsFileWithCdn("~/js/signin/demo.js"),
                         //new JsFileWithCdn("~/scripts/site/table-advanced.js"),
                        //new JsFileWithCdn("~/scripts/site/index3.js"),
                        //new JsFileWithCdn("~/scripts/site/tasks.js")

                        new JsFileWithCdn("~/js/app.js"),
                        new JsFileWithCdn("~/js/app.config.js"),
                        new JsFileWithCdn("~/js/app.route.js"),
                        new JsFileWithCdn("~/js/modules/displayTime.js"),

                        new JsFileWithCdn("~/js/components/quiz/stopwatch.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.filter.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.directive.js"),


                        new JsFileWithCdn("~/js/components/userdetails/userdetails.module.js"),
                        new JsFileWithCdn("~/js/components/user/userdetails.controller.js"),
                        new JsFileWithCdn("~/js/components/user/user.controller.js"),
                        new JsFileWithCdn("~/js/components/user/account.controller.js"),
                        new JsFileWithCdn("~/js/components/user/user.service.js"),
                        new JsFileWithCdn("~/js/components/user/updates.service.js"),
                        new JsFileWithCdn("~/js/components/dashboard/dashboard.service.js"),
                        new JsFileWithCdn("~/js/components/dashboard/dashboard.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/sidemenu.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/createClass.controller.js"),

                        new JsFileWithCdn("~/js/components/box/box.controller.js"),
                        new JsFileWithCdn("~/js/components/box/feed.controller.js"),
                        new JsFileWithCdn("~/js/components/box/item.controller.js"),
                        new JsFileWithCdn("~/js/components/box/quizzes.controller.js"),
                        new JsFileWithCdn("~/js/components/box/members.controller.js"),

                  
                        new JsFileWithCdn("~/js/components/search.controller.js"),

                        new JsFileWithCdn("~/js/components/library/library.controller.js"),

                        new JsFileWithCdn("~/js/components/item/item.controller.js"),
                        new JsFileWithCdn("~/js/components/item/item.service.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.service.js"),
                        new JsFileWithCdn("~/js/components/app.controller.js"),
                        new JsFileWithCdn("~/js/services/ajaxService.js"),

                        new JsFileWithCdn("~/js/shared/colorOnLength.js"),
                        new JsFileWithCdn("~/js/shared/loader.js"),
                        new JsFileWithCdn("~/js/shared/mixitup.js"),
                        new JsFileWithCdn("~/js/shared/userimage.js"),
                        new JsFileWithCdn("~/js/shared/megaNumbers.js"),
                        new JsFileWithCdn("~/js/shared/focusMe.js"),


                        new JsFileWithCdn("~/js/components/item/upload.controller.js"),
                        new JsFileWithCdn("~/js/shared/dropbox.js"),
                        new JsFileWithCdn("~/js/shared/google.js")
                    }

                },
                {
                    "contactUs", new[]
                    {
                         new JsFileWithCdn("~/scripts/site/gmaps.js"),
                          new JsFileWithCdn("~/js/contact-us.js")
                    }
                }
            };



            return jsDictionary;
        }

    }
}