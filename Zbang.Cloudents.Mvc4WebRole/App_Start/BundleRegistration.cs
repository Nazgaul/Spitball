﻿using System.Collections.Generic;

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
                //    "newrtl3", new[]
                //    {
                //        "~/Content/GeneralRtl.css",
                //        "~/Content/rtl3.css",
                //        "~/Content/StoreRtl.css",
                //        "~/Content/SetupSchoolRtl.css",
                //        "~/Content/HeaderRtl.css",
                //        "~/Content/BoxFeedRtl.css",
                //        "~/Content/ItemRtl.css",                       
                //        "~/Content/HomeRtl.css"
                //    }
                //},
                {
                    "newcore3", new[]
                    {
                        "~/Content/Normalize.css",
                        "~/Content/General.css",
                        "~/Content/Header.css",
                        "~/Content/Site3.css",
                        "~/Content/SVG.css",
                        "~/Content/Animations.css",
                        "~/Content/UserPage.css",
                        "~/Content/Search.css",
                        "~/Content/Sidebar.css",
                        "~/Content/SetupSchool.css",
                        "~/Content/Modal.css",
                        "~/Content/BoxFeed.css",
                        "~/Content/Quiz.css",
                        "~/Content/Invite.css",
                        "~/Content/Upload.css",
                        "~/Content/Box3.css",
                        "~/Content/Item.css",
                        "~/Content/Settings.css",
                        "~/Content/DashLib.css",
                        "~/Content/Store.css",
                        "~/Content/Home.css",
                        "~/Content/jquery.mCustomScrollbar.css",
                        "~/Content/Letter.css",
                        "~/Content/RtlFix.css",
                        "~/Content/textAngular.css"
                    }
                },
                //{
                //    "staticRtl", new[]
                //    {
                //        "~/Content/GeneralRtl.css",
                //        "~/Content/StaticRtl.css",
                //        "~/Content/HeaderRtl.css"
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
                }
            };
            return cssDictionary;
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>
            {
                {
                    "angular", new[]
                    {
                        new JsFileWithCdn("~/scripts/jquery.min.js",
                            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                        new JsFileWithCdn("~/scripts/underscore.js"),
                        new JsFileWithCdn("~/scripts/angular.min.js",
                            "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular.min.js"),
                        new JsFileWithCdn("~/scripts/angular-route.js"
                            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-route.min.js"
                            ),
                        new JsFileWithCdn("~/scripts/angular-cookies.js"
                            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-cookies.min.js"
                            ),
                            new JsFileWithCdn("~/scripts/angular-messages.js"
                            //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-messages.min.js"
                            ),
                        //new JsFileWithCdn("~/scripts/angular-sanitize.js"
                        //    //"https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-sanitize.min.js"
                        //    ),
                        new JsFileWithCdn("~/scripts/angular-animate.js",
                            "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-animate.min.js"),
                            new JsFileWithCdn("~/scripts/textAngular/textAngular-rangy.min.js"),
                            new JsFileWithCdn("~/scripts/textAngular/textAngular-sanitize.js"),
                            new JsFileWithCdn("~/scripts/textAngular/textAngularSetup.js"),
                            new JsFileWithCdn("~/scripts/textAngular/textAngular.js"),


                        new JsFileWithCdn("~/scripts/angulartics.js"), new JsFileWithCdn("~/scripts/angulartics-ga.js"),
                        //new JsFileWithCdn("~/scripts/angular-appinsights.js"),
                        new JsFileWithCdn("~/scripts/angular-cache-2.4.1.js"),
                        new JsFileWithCdn("~/scripts/stacktrace.js"),              
                        new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),
                        //new JsFileWithCdn("~/scripts/bindonce.js"),
                        new JsFileWithCdn("~/scripts/ui-bootstrap-custom-tpls-0.12.0.js"),
                        new JsFileWithCdn("~/scripts/angular-draganddrop.js"),
                        new JsFileWithCdn("~/scripts/angular-debounce.js"),
                        new JsFileWithCdn("~/scripts/jquery.mousewheel.min.js"),
                        new JsFileWithCdn("~/scripts/jquery.mCustomScrollbar.js"),
                        new JsFileWithCdn("~/scripts/Modernizr.js"),

                        new JsFileWithCdn("~/scripts/plupload2/moxie.js"),
                        new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),
                        new JsFileWithCdn("~/scripts/plupload2/angular-plupload.js"),
                        new JsFileWithCdn("~/scripts/CountUp.js"),
                        new JsFileWithCdn("~/scripts/elastic.js"),
                        new JsFileWithCdn("~/scripts/angular-mcustomscrollbar.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js"),

                        new JsFileWithCdn("~/js/modules/displayTime.js"),

                        new JsFileWithCdn("~/js/modules/angular-timer.js"),
                        //new JsFileWithCdn("/js/modules/lazySrc.js"),                        
                        new JsFileWithCdn("~/js/modules/wizard.js"),
                        new JsFileWithCdn("~/js/modules/textDirection.js"),

                        new JsFileWithCdn("~/js/app.js"),

                        new JsFileWithCdn("/js/controllers/general/mainCtrl.js"),
                        new JsFileWithCdn("/js/controllers/general/shareCtrl.js"),                        
                        new JsFileWithCdn("/js/controllers/general/uploadListCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/settingsCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/loginWrapperCtrl.js"),                        
                        new JsFileWithCdn("/js/controllers/account/accountCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/notificationsCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/congratsCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/notificationSettingsCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/userDetailsCtrl.js"),
                        new JsFileWithCdn("/js/controllers/account/loginCtrl.js"),
                        new JsFileWithCdn("/js/controllers/search/searchCtrl.js"),                        
                        new JsFileWithCdn("/js/controllers/box/boxCtrl.js"),
                        new JsFileWithCdn("/js/controllers/box/sideBarCtrl.js"),
                        new JsFileWithCdn("/js/controllers/box/leavePromptCtrl.js"),
                        new JsFileWithCdn("/js/controllers/box/boxTabsCtrl.js"),
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
                        new JsFileWithCdn("/js/controllers/dashboard/inviteCloudentsCtrl.js"),
                        new JsFileWithCdn("/js/controllers/library/libraryCtrl.js"),
                        new JsFileWithCdn("/js/controllers/library/libChooseCtrl.js"),
                        new JsFileWithCdn("/js/controllers/library/createDepartmentCtrl.js"),
                        new JsFileWithCdn("/js/controllers/library/restrictionPopUpCtrl.js"),
                        new JsFileWithCdn("/js/controllers/library/libraryRenameCtrl.js"),
                        new JsFileWithCdn("/js/controllers/user/userCtrl.js"),
                        new JsFileWithCdn("/js/controllers/item/itemCtrl.js"),
                        new JsFileWithCdn("/js/controllers/item/itemRegCtrl.js"),                        
                        new JsFileWithCdn("/js/controllers/item/itemFullScreenCtrl.js"),
                        new JsFileWithCdn("/js/controllers/item/itemRenameCtrl.js"),
                        new JsFileWithCdn("/js/controllers/item/itemFlagCtrl.js"),
                        new JsFileWithCdn("/js/controllers/quiz/quizCtrl.js"),
                        new JsFileWithCdn("/js/controllers/quiz/challengeCtrl.js"),
                        new JsFileWithCdn("/js/controllers/quiz/quizCreateCtrl.js"),
                        new JsFileWithCdn("/js/controllers/quiz/quizCloseCtrl.js"),
                        new JsFileWithCdn("/js/services/ajaxService.js"),
                        new JsFileWithCdn("/js/services/modalWrapper.js"),
                        new JsFileWithCdn("/js/services/versionChecker.js"),
                        new JsFileWithCdn("/js/services/login.js"),
                        new JsFileWithCdn("/js/services/dropbox.js"),
                        new JsFileWithCdn("/js/services/google.js"),
                        new JsFileWithCdn("/js/services/qna.js"),
                        new JsFileWithCdn("/js/services/notificationManager.js"),
                        new JsFileWithCdn("/js/services/upload.js"),
                        new JsFileWithCdn("/js/services/newUpdates.js"),
                        new JsFileWithCdn("/js/services/focus.js"),
                        new JsFileWithCdn("/js/services/box.js"),
                        new JsFileWithCdn("/js/services/item.js"),
                        new JsFileWithCdn("/js/services/library.js"),
                        new JsFileWithCdn("~/js/services/userPoints.js"),
                        new JsFileWithCdn("/js/services/account.js"),
                        new JsFileWithCdn("/js/services/share.js"),
                        new JsFileWithCdn("/js/directives/boxLogo.js"),
                        //new JsFileWithCdn("/js/services/htmlCache.js"),                        
                        new JsFileWithCdn("/js/services/resourceManager.js"),
                        new JsFileWithCdn("/js/services/notificationHandler.js"),
                        new JsFileWithCdn("/js/services/library.js"),
                        new JsFileWithCdn("/js/services/search.js"),
                        new JsFileWithCdn("/js/services/quiz.js"),
                        new JsFileWithCdn("/js/services/dashboard.js"),
                        new JsFileWithCdn("/js/services/user.js"),
                        new JsFileWithCdn("/js/services/facebook.js"),
                        new JsFileWithCdn("/js/services/userDetails.js"),   
                        new JsFileWithCdn("/js/services/storeHandler.js"),   
                        new JsFileWithCdn("/js/services/stacktrace.js"),
                        new JsFileWithCdn("/js/directives/ngMatch.js"),
                        new JsFileWithCdn("/js/directives/searchCoords.js"),
                        new JsFileWithCdn("/js/directives/userimage.js"),
                        new JsFileWithCdn("/js/directives/fileReader.js"),
                        new JsFileWithCdn("/js/directives/ngPlaceholder.js"),
                        new JsFileWithCdn("/js/directives/storageSpace.js"),
                        new JsFileWithCdn("/js/directives/dropZone.js"),
                        new JsFileWithCdn("/js/directives/fbBlock.js"),
                        new JsFileWithCdn("/js/directives/letter.js"),
                        new JsFileWithCdn("/js/directives/loadSpinner.js"),
                        new JsFileWithCdn("~/js/directives/userCoupon.js"),
                        new JsFileWithCdn("/js/directives/scrollToTop.js"),
                        new JsFileWithCdn("/js/directives/focusForm.js"),


                        new JsFileWithCdn("/js/directives/homePageScroll.js"),
                        new JsFileWithCdn("/js/directives/itemFullscreen.js"),
                        new JsFileWithCdn("/js/directives/itemScroll.js"),
                        new JsFileWithCdn("/js/directives/mLoader.js"),
                        new JsFileWithCdn("/js/directives/login.js"),
                        new JsFileWithCdn("/js/directives/quizGraph.js"),
                        new JsFileWithCdn("/js/directives/homeVideo.js"),
                        new JsFileWithCdn("/js/directives/focusOn.js"),
                        new JsFileWithCdn("~/js/directives/countTo.js"),
                        new JsFileWithCdn("~/js/directives/userPoints.js"),
                        new JsFileWithCdn("~/js/directives/userTooltip.js"),
                        new JsFileWithCdn("~/js/directives/userDetails.js"),
                        new JsFileWithCdn("~/js/directives/departmentsTooltip.js"),
                        new JsFileWithCdn("/js/directives/moxieDropzone.js"),
                        new JsFileWithCdn("/js/directives/facebookFeed.js"),
                        new JsFileWithCdn("/js/directives/selectOnClick.js"),
                        new JsFileWithCdn("/js/directives/facebookFeed.js"),
                        new JsFileWithCdn("/js/directives/contentEditable.js"),
                        new JsFileWithCdn("/js/directives/rateStar.js"),
                        new JsFileWithCdn("/js/directives/boxItemTooltip.js"),                        
                        new JsFileWithCdn("/js/filters/escapeHtmlChars.js"),
                        new JsFileWithCdn("/js/filters/kNumber.js"),
                        new JsFileWithCdn("/js/filters/defaultImage.js"),
                        new JsFileWithCdn("/js/filters/fileSize.js"),
                        new JsFileWithCdn("/js/filters/extToClass.js"),                        
                        new JsFileWithCdn("/js/filters/actionText.js"),
                        new JsFileWithCdn("/js/filters/highlight.js"),
                        new JsFileWithCdn("/js/filters/orderBy.js"),
                        new JsFileWithCdn("/js/filters/stringFormat.js"),
                        new JsFileWithCdn("/js/controllers/store/storeCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/productCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/contactCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/couponCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/viewCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/checkoutCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/categoryCtrl.js"),
                        new JsFileWithCdn("/js/controllers/store/carouselCtrl.js"),
                        new JsFileWithCdn("/js/services/store.js"),

                        new JsFileWithCdn("/js/directives/store/productsMenu.js"),
                        new JsFileWithCdn("/js/directives/misc.js"),
                        new JsFileWithCdn("/js/directives/store/categoryLink.js"),

                        new JsFileWithCdn("/js/filters/store/percentage.js")
                    }
                },
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
                {
                    "General", new[]
                    {
                        new JsFileWithCdn("~/scripts/jquery.min.js",
                            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                        //new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
                        //new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
                        //new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
                        //new JsFileWithCdn("~/scripts/Modernizr.js"),

                        ////new JsFileWithCdn("~/Scripts/MutationObserver.js"),

                        //new JsFileWithCdn("~/Js/Utils2.js"),
                        //new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

                        //new JsFileWithCdn("~/Js/pubsub2.js"),
                        //new JsFileWithCdn("~/scripts/svg4everybody.js")
                    }
                },
                {
                    "faq", new[]
                    {
                        new JsFileWithCdn("~/scripts/jquery.min.js",
                            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                    }
                }
            };


            //jsDictionary.Add("homeMobile",new[] {
            //    new JsFileWithCdn("~/Js/Mobile/Logon.js"),
            //    new JsFileWithCdn("~/Js/Mobile/Welcome.js")
            //    });

            //jsDictionary.Add("MChooseLib",new[] {
            //    new JsFileWithCdn("~/Js/Mobile/MLibraryChoose.js")
            //    });


            //jsDictionary.Add("mobileItem", new JsFileWithCdn("~/Scripts/jquery-2.1.1.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
            //    //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
            //                        new JsFileWithCdn("~/Js/Utils2.js"),
            //                        new JsFileWithCdn("~/Js/pubsub2.js"),
            //                        new JsFileWithCdn("~/Js/Cache2.js"),
            //                        new JsFileWithCdn("~/Js/DataContext2.js"),
            //                        new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js"));

            return jsDictionary;
        }

    }
}