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


                ["homePage"] = new[]
                    {
                        "~/content/site/general.css",
                        "~/content/homepage/homePage2.css",
                        "~/content/site/staticPage.css",
                        "~/content/site/layout.css",
                        "~/content/signin/custom.css",
                        "~/content/jquery.bxslider.css"
                    },


                ["staticPage"] = new[]
                    {
                        "~/content/site/general.css",
                        "~/content/site/staticPage.css",
                        "~/Content/homepage/bootstrap.css", //TODO: check if we need this.
                        "~/content/site/layout.css",
                        "~/content/site/itemGallery.css"
                    },

                ["signin"] = new[] // passwordUpdate uses it - this page will be modified in v4.
                    {
                        "~/Content/homepage/bootstrap.css", //TODO: check if we need this.
                        "~/Content/signin/login-soft.css",
                        "~/Content/signin/components-md.css",
                        "~/Content/signin/plugins-md.css",
                        "~/Content/site/layout.css",
                        "~/Content/signin/default.css",
                        "~/Content/signin/custom.css"
                    },

                ["site4"] = new[]
                    {
                        "~/content/site/angularWithChanges2.css",
                        "~/bower_components/textAngular/dist/textAngular.css",
                        "~/bower_components/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css",
                        "~/content/site/inline-material.css",
                        "~/content/bootstrap/bootstrap.css",
                        "~/content/site/profile.css",//
                        "~/content/site/components-rounded.css",//
                        "~/content/site/layout.css",//
                        "~/content/site/light.css",//
                        "~/content/site/menu.css",
                        "~/content/site/chat.css",
                        "~/content/site/header.css",
                        "~/content/site/general.css", // this should be on top
                        "~/content/site/angularMaterialOverride.css",
                        "~/content/site/dashboard.css",
                        "~/content/site/box.css",
                        "~/content/site/feed.css",
                        "~/content/site/user.css",
                        "~/content/site/accountSettings.css",
                        "~/content/site/library.css",
                        "~/content/site/item.css",
                        "~/content/site/quiz.css",
                        "~/content/site/quizCreate.css",
                        "~/content/site/share.css",
                        "~/content/site/leaderboard.css",
                        "~/content/site/search.css",
                        "~/content/site/libraryChoose.css",
                        "~/content/site/error.css",
                        "~/content/site/accordion.css",
                        "~/content/site/themedark.css",
                        "~/content/site/themelight.css"
                    }

            };
            return cssDictionary;
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>
            {


                {
                    "homePage", new[]
                    {
                        new JsFileWithCdn("~/Scripts/jquery-2.2.0.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"),
                        new JsFileWithCdn("~/Scripts/jquery.validate.js"),
                        new JsFileWithCdn("~/js/signin/login-soft.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js"),
                        new JsFileWithCdn("~/scripts/waypoints.min.js"),
                        new JsFileWithCdn("~/Scripts/jquery.bxslider.js"),
                        new JsFileWithCdn("~/Scripts/CountUp.js"),
                        new JsFileWithCdn("~/js/shared/languageHandler.js"),
                        new JsFileWithCdn("~/Js/HomePage.js")
                    }
                },
                {
                    "staticPage", new[]
                    {
                        new JsFileWithCdn("~/Scripts/jquery-2.2.0.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"),
                        new JsFileWithCdn("~/scripts/jquery.bxslider.js"),
                        new JsFileWithCdn("~/js/signin/bootstrap.min.js"),
                        new JsFileWithCdn("~/js/shared/languageHandler.js"),
                        new JsFileWithCdn("~/js/signin/staticPages.js"),
                        new JsFileWithCdn("~/js/signin/itemGallery.js")

                    }
                },
                 {
                    //TODO: maybe we can remove this??? // passwordUpdate uses it - this page will be modified in v4.
                    "signin", new[]
                    {


                        new JsFileWithCdn("~/Scripts/jquery-2.2.0.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"),
                        new JsFileWithCdn("~/Scripts/jquery.validate.js"),
                        new JsFileWithCdn("~/js/signin/metronic.js"),
                        new JsFileWithCdn("~/js/signin/layout.js"),
                        new JsFileWithCdn("~/js/signin/login-soft.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js")
                    }
                },
                
                {
                    "site4", new[]
                    {

                        new JsFileWithCdn("~/Scripts/jquery-2.2.0.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"),
                        new JsFileWithCdn("~/scripts/Modernizr.js"),
                        new JsFileWithCdn("~/scripts/angular.js","https://ajax.googleapis.com/ajax/libs/angularjs/1.5.8/angular.min.js"),
                        new JsFileWithCdn("~/Scripts/angular-ui-router.js"),
                        new JsFileWithCdn("~/Scripts/angular-messages.js"),
                        //new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular-rangy.min.js"),
                        new JsFileWithCdn("~/bower_components/angular-cache/dist/angular-cache.js"),
                        new JsFileWithCdn("~/Scripts/angular-sanitize.js"),

                        new JsFileWithCdn("~/bower_components/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"),
                        new JsFileWithCdn("~/bower_components/ng-scrollbars/src/scrollbars.js"),

                        new JsFileWithCdn("~/bower_components/angular-google-analytics/dist/angular-google-analytics.js"),
                        new JsFileWithCdn("~/bower_components/oclazyload/dist/oclazyload.min.js"),

                        //new JsFileWithCdn("~/scripts/textAngularSetup.js"),
                        //new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular.js"),

                        new JsFileWithCdn("~/Scripts/angular-animate.js"),
                        new JsFileWithCdn("~/Scripts/angular-aria.js"),
                        new JsFileWithCdn("~/Scripts/angular-material/angular-material.js", "https://ajax.googleapis.com/ajax/libs/angular_material/1.0.9/angular-material.min.js"),

                        new JsFileWithCdn("~/scripts/angular-srph-infinite-scroll.js"),


                        new JsFileWithCdn("~/scripts/plupload2/moxie.min.js"),
                        new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),
                        new JsFileWithCdn("~/scripts/plupload2/angular-plupload2.js"),

                        new JsFileWithCdn("~/scripts/ui-bootstrap-custom-tpls-1.2.1.min.js"),
                        new JsFileWithCdn("~/scripts/svg4everybodyAngular.js"),
                        new JsFileWithCdn("~/scripts/angular-countUp.js"),
                        new JsFileWithCdn("~/scripts/draganddrop.js"),
                        new JsFileWithCdn("~/scripts/angular-dfp.js"),
                        new JsFileWithCdn("~/bower_components/angular-timeago/dist/angular-timeago.min.js"),
                        new JsFileWithCdn("~/js/polyfills.js"),

                        new JsFileWithCdn("~/js/app.js"),
                        new JsFileWithCdn("~/js/app.config.js"),
                        new JsFileWithCdn("~/js/doubleclick.config.js"),
                        new JsFileWithCdn("~/js/shared/languageHandler.js"),
                        new JsFileWithCdn("~/js/routerHelperProvider.js"),
                        new JsFileWithCdn("~/js/app.route.js"),


                        new JsFileWithCdn("~/js/components/quiz/stopwatch.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.filter.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.directive.js"),
                        new JsFileWithCdn("~/js/components/quiz/quizCreate1.config.js"),
                        
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.challenge.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.score.module.js"),
                        new JsFileWithCdn("~/js/components/userdetails/userdetails.module.js"),
                        new JsFileWithCdn("~/js/components/box/box.module.js"),

                        new JsFileWithCdn("~/js/components/user/userdetails.controller.js"),
                        new JsFileWithCdn("~/js/components/user/user.controller.js"),
                        new JsFileWithCdn("~/js/components/user/user.routes.js"),

                        new JsFileWithCdn("~/js/components/user/user.service.js"),
                        new JsFileWithCdn("~/js/components/user/updates.service.js"),

                        new JsFileWithCdn("~/js/components/dashboard/dashboard.service.js"),

                        new JsFileWithCdn("~/js/components/dashboard/dashboard.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/dashboard.routes.js"),

                        new JsFileWithCdn("~/js/components/dashboard/university.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/createBox.controller.js"),


                        new JsFileWithCdn("~/js/menu/menulink.directive.js"),
                        new JsFileWithCdn("~/js/menu/sidemenu.controller.js"),
                       
                        //new JsFileWithCdn("~/js/menu/nospace.filter.js"),


                        new JsFileWithCdn("~/js/components/box/box.controller.js"),
                        new JsFileWithCdn("~/js/components/box/tab.controller.js"),
                        new JsFileWithCdn("~/js/components/box/box.routes.js"),
                        new JsFileWithCdn("~/js/components/box/shareBox.directive.js"),
                        new JsFileWithCdn("~/js/components/box/feed.controller.js"),
                        new JsFileWithCdn("~/js/components/box/feed.likes.controller.js"),
                        new JsFileWithCdn("~/js/components/box/item.controller.js"),
                        new JsFileWithCdn("~/js/components/box/quizzes.controller.js"),
                        new JsFileWithCdn("~/js/components/box/members.controller.js"),
                        new JsFileWithCdn("~/js/components/box/box.service.js"),
                        new JsFileWithCdn("~/js/components/box/recommended.controller.js"),
                        new JsFileWithCdn("~/js/components/box/slideit.directive.js"),


                        new JsFileWithCdn("~/js/components/account/account.controller.js"),

                        new JsFileWithCdn("~/js/components/account/notification.controller.js"),
                        new JsFileWithCdn("~/js/components/account/password.controller.js"),
                        new JsFileWithCdn("~/js/components/account/info.controller.js"),
                        new JsFileWithCdn("~/js/components/account/account.service.js"),
                        new JsFileWithCdn("~/js/components/account/account.routes.js"),
                        new JsFileWithCdn("~/js/components/account/unregister.controller.js"),
                        new JsFileWithCdn("~/js/components/account/unregisterShow.directive.js"),
                        new JsFileWithCdn("~/js/components/account/department.controller.js"),

                        new JsFileWithCdn("~/js/components/search/search.controller.js"),
                        new JsFileWithCdn("~/js/components/search/searchTrigger.controller.js"),
                        new JsFileWithCdn("~/js/components/search/search.service.js"),


                        new JsFileWithCdn("~/js/components/leaderboard.controller.js"),

                        new JsFileWithCdn("~/js/components/library/library.controller.js"),
                        new JsFileWithCdn("~/js/components/library/libraryChoose.controller.js"),
                        new JsFileWithCdn("~/js/components/library/library.routes.js"),
                        new JsFileWithCdn("~/js/components/library/library.service.js"),
                        new JsFileWithCdn("~/js/components/library/countryService.service.js"),
                        new JsFileWithCdn("~/js/components/library/universityCover.directive.js"),

                        new JsFileWithCdn("~/js/components/share/invite.controller.js"),
                        new JsFileWithCdn("~/js/components/share/share.service.js"),


                        new JsFileWithCdn("~/js/components/item/item.controller.js"),
                        new JsFileWithCdn("~/js/components/item/comments.controller.js"),
                        new JsFileWithCdn("~/js/components/item/item.service.js"),

                        new JsFileWithCdn("~/js/components/quiz/quiz.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.challenge.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.score.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.service.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.routes.js"),
                        //new JsFileWithCdn("~/js/components/quiz/quizCreate.controller.js"),

                        new JsFileWithCdn("~/js/components/app.controller.js"),
                        new JsFileWithCdn("~/js/shared/ajaxService.js"),
                        new JsFileWithCdn("~/js/shared/ajaxService2.js"),

                        new JsFileWithCdn("~/js/shared/colorOnLength.js"),
                        new JsFileWithCdn("~/js/shared/loader.js"),
                        new JsFileWithCdn("~/js/shared/userimage.js"),
                        new JsFileWithCdn("~/js/shared/megaNumbers.js"),
                        new JsFileWithCdn("~/js/shared/focusMe.js"),
                        new JsFileWithCdn("~/js/shared/userDetails.js"),
                        new JsFileWithCdn("~/js/shared/fileReader.js"),
                        new JsFileWithCdn("~/js/shared/animationClass.js"),
                        new JsFileWithCdn("~/js/shared/removeKeyboard.js"),
                        new JsFileWithCdn("~/js/shared/firstLetter.js"),
                        new JsFileWithCdn("~/js/shared/history.js"),
                        new JsFileWithCdn("~/js/shared/itemThumbnail.js"),
                        new JsFileWithCdn("~/js/shared/notification.js"),
                        new JsFileWithCdn("~/js/shared/pagingOnScroll.directive.js"),

                        new JsFileWithCdn("~/js/components/item/upload.controller.js"),
                        new JsFileWithCdn("~/js/components/item/externalProviderUpload.service.js"),

                        new JsFileWithCdn("~/js/shared/dropbox.js"),
                        new JsFileWithCdn("~/js/shared/google.js"),
                        new JsFileWithCdn("~/js/shared/facebook.js"),
                        new JsFileWithCdn("~/js/shared/resourceManager.js"),
                        new JsFileWithCdn("~/js/shared/versionChecker.js"),
                        new JsFileWithCdn("~/js/shared/intercom.js"),
                        new JsFileWithCdn("~/js/shared/inlineManualNew.js"),
                        new JsFileWithCdn("~/js/shared/scrollToTop.directive.js"),
                        new JsFileWithCdn("~/js/shared/collapseHeader.directive.js"),
                        new JsFileWithCdn("~/js/shared/keyboardAction.directive.js"),
                        new JsFileWithCdn("~/js/shared/menuAd.directive.js"),

                        new JsFileWithCdn("~/Scripts/jquery.signalR-2.2.0.js"),
                        new JsFileWithCdn("~/Scripts/angular-signalr-hub.js"),

                        new JsFileWithCdn("~/js/components/chat/chat.controller.js"),
                        new JsFileWithCdn("~/js/components/chat/chat.factory.js"),
                        new JsFileWithCdn("~/js/components/chat/hubFactory.js"),
                        new JsFileWithCdn("~/js/components/chat/chatTimeAgo.js"),
                        new JsFileWithCdn("~/js/components/chat/previewController.js"),
                        new JsFileWithCdn("~/js/components/chat/toggleChat.directive.js"),
                        new JsFileWithCdn("~/js/components/chat/hideChatOnMobile.directive.js"),



                    }

                }
            };



            return jsDictionary;
        }

    }
}