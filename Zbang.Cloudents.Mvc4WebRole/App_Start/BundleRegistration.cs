using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class BundleRegistration
    {
        public static void RegisterBundles()
        {
            BundleConfig.RegisterBundle(RegisterCss(), RegisterJs());
            //BundleConfig.RegisterBundles(RegisterCss(), RegisterJs());
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<CssWithRtl>>> RegisterCss()
        {
            var cssDictionary = new Dictionary<string, IEnumerable<CssWithRtl>>
            {


                ["homePage"] = new[]
                    {
                        new CssWithRtl("~/content/site/GeneralWithStatic.css","~/content/site/GeneralWithStatic.rtl.css"),
                        new CssWithRtl("~/content/homepage/homePage2.css","~/content/homepage/homePage2.rtl.css"),
                        new CssWithRtl("~/content/site/staticPage.css","~/content/site/staticPage.rtl.css"),
                        new CssWithRtl("~/content/site/layout.css","~/content/site/layout.rtl.css"),
                        new CssWithRtl("~/content/signin/custom.css","~/content/signin/custom.rtl.css"),
                        new CssWithRtl("~/content/jquery.bxslider.css","~/content/jquery.bxslider.rtl.css")
                    },


                ["staticPage"] = new[]
                    {
                        new CssWithRtl("~/content/site/GeneralWithStatic.css"),
                        new CssWithRtl("~/content/site/staticPage.css"),
                        new CssWithRtl("~/Content/homepage/bootstrap.css"), //TODO: check if we need this.
                        new CssWithRtl("~/content/site/layout.css"),
                        new CssWithRtl("~/content/site/itemGallery.css")
                    },

                ["signin"] = new[] // passwordUpdate uses it - this page will be modified in v4.
                    {
                        new CssWithRtl("~/Content/homepage/bootstrap.css"), //TODO: check if we need this.
                        new CssWithRtl("~/Content/signin/login-soft.css"),
                        new CssWithRtl("~/Content/signin/components-md.css"),
                        new CssWithRtl("~/Content/signin/plugins-md.css"),
                        new CssWithRtl("~/Content/site/layout.css"),
                        new CssWithRtl("~/Content/signin/default.css"),
                        new CssWithRtl("~/Content/signin/custom.css")
                    },

                ["site4"] = new[]
                    {

                        //new CssWithRtl("~/content/angular-material.css","~/content/angular-material.css"),
                        new CssWithRtl("~/bower_components/textAngular/dist/textAngular.css"),
                        new CssWithRtl("~/bower_components/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css"),
                        new CssWithRtl("~/content/site/inline-material.css"),
                        new CssWithRtl("~/content/bootstrap/bootstrap.css"),
                        new CssWithRtl("~/content/site/profile.css"),//
                        new CssWithRtl("~/content/site/components-rounded.css"),//
                        new CssWithRtl("~/content/site/layout.css"),//
                        new CssWithRtl("~/content/site/light.css"),//
                        new CssWithRtl("~/content/site/menu.css"),
                        new CssWithRtl("~/content/site/chat.css"),
                        new CssWithRtl("~/content/site/header.css"),
                        new CssWithRtl("~/content/site/GeneralWithStatic.css"),
                        new CssWithRtl("~/content/site/general.css"), // this should be on top
                        new CssWithRtl("~/content/site/angularMaterialOverride.css"),
                        new CssWithRtl("~/content/site/dashboard.css"),
                        new CssWithRtl("~/content/site/box.css"),
                        new CssWithRtl("~/content/site/feed.css"),
                        new CssWithRtl("~/content/site/user.css"),
                        new CssWithRtl("~/content/site/accountSettings.css"),
                        new CssWithRtl("~/content/site/library.css"),
                        new CssWithRtl("~/content/site/item.css"),
                        new CssWithRtl("~/content/site/quiz.css"),
                        new CssWithRtl("~/content/site/quizCreate.css"),
                        new CssWithRtl("~/content/site/share.css"),
                        new CssWithRtl("~/content/site/leaderboard.css"),
                        new CssWithRtl("~/content/site/search.css"),
                        new CssWithRtl("~/content/site/libraryChoose.css"),
                        new CssWithRtl("~/content/site/error.css"),
                        new CssWithRtl("~/content/site/accordion.css"),
                        new CssWithRtl("~/content/site/selectClass.css"),
                    }

            };
            return cssDictionary;
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>
            {


                {
                    "jsHomePage", new[]
                    {
                        new JsFileWithCdn("~/scripts/jquery-2.2.0.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"),
                        new JsFileWithCdn("~/scripts/jquery.validate.js"),
                        new JsFileWithCdn("~/js/signin/login-soft.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js"),
                        new JsFileWithCdn("~/scripts/waypoints.min.js"),
                        new JsFileWithCdn("~/scripts/jquery.bxslider.js"),
                        new JsFileWithCdn("~/scripts/CountUp.js"),
                        new JsFileWithCdn("~/js/shared/languageHandler.js"),
                        new JsFileWithCdn("~/js/HomePage.js")
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
                    "jsSite4", new[]
                    {

                        new JsFileWithCdn("~/Scripts/jquery-2.2.0.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.2.0/jquery.min.js"),
                        new JsFileWithCdn("~/scripts/Modernizr.js"),
                        new JsFileWithCdn("~/scripts/angular.js","https://ajax.googleapis.com/ajax/libs/angularjs/1.5.8/angular.min.js"),
                        new JsFileWithCdn("~/Scripts/angular-ui-router.js"),
                        new JsFileWithCdn("~/Scripts/angular-messages.js"),
                        new JsFileWithCdn("~/bower_components/angular-cache/dist/angular-cache.js"),
                        new JsFileWithCdn("~/Scripts/angular-sanitize.js"),

                        new JsFileWithCdn("~/bower_components/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"),
                        new JsFileWithCdn("~/bower_components/ng-scrollbars/src/scrollbars.js"),

                        new JsFileWithCdn("~/bower_components/angular-google-analytics/dist/angular-google-analytics.js"),
                        new JsFileWithCdn("~/bower_components/oclazyload/dist/oclazyload.js"),
                        new JsFileWithCdn("~/bower_components/angular-vs-repeat/src/angular-vs-repeat.js"),
                        new JsFileWithCdn("~/Scripts/angular-animate.js"),
                        new JsFileWithCdn("~/Scripts/angular-aria.js"),
                        new JsFileWithCdn("~/Scripts/angular-material/angular-material.js", "https://ajax.googleapis.com/ajax/libs/angular_material/1.1.1/angular-material.min.js"),

                        new JsFileWithCdn("~/scripts/angular-srph-infinite-scroll.js"),

                        new JsFileWithCdn("~/Scripts/jquery.signalR-2.2.0.js"),
                        new JsFileWithCdn("~/Scripts/angular-signalr-hub.js"),

                        //TODO: move to oclazy in box
                        new JsFileWithCdn("~/bower_components/plupload/js/moxie.min.js"),
                        new JsFileWithCdn("~/bower_components/plupload/js/plupload.dev.js"),
                        new JsFileWithCdn("~/bower_components/angular-plupload/src/angular-plupload.js"),
                        new JsFileWithCdn("~/scripts/draganddrop.js"),

                        new JsFileWithCdn("~/scripts/ui-bootstrap-custom-tpls-1.2.1.min.js"),
                        //new JsFileWithCdn("~/scripts/svg4everybodyAngular.js"),
                         //TODO: move to oclazy in user
                        new JsFileWithCdn("~/scripts/angular-countUp.js"),

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
                        new JsFileWithCdn("~/js/components/userdetails/userdetails.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.challenge.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.score.module.js"),
                        //new JsFileWithCdn("~/js/components/box/box.module.js"),


                        new JsFileWithCdn("~/js/components/quiz/stopwatch.filter.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.directive.js"),

                        new JsFileWithCdn("~/js/components/quiz/quizCreate1.config.js"),
                        //new JsFileWithCdn("~/js/components/box/upload1.config.js"),

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


                    new JsFileWithCdn("~/js/components/box/box.module.js"),
                     new JsFileWithCdn("~/js/components/box/box.controller.js"),
                     new JsFileWithCdn("~/js/components/box/boxSettings.controller.js"),
                     new JsFileWithCdn("~/js/components/box/tab.controller.js"),
                     new JsFileWithCdn("~/js/components/box/shareBox.directive.js"),
                     new JsFileWithCdn("~/js/components/box/feed.controller.js"),
                     new JsFileWithCdn("~/js/components/box/feed.likes.controller.js"),
                     new JsFileWithCdn("~/js/components/box/item.controller.js"),
                     new JsFileWithCdn("~/js/components/box/quizzes.controller.js"),
                     new JsFileWithCdn("~/js/components/box/members.controller.js"),

                     new JsFileWithCdn("~/js/components/box/recommended.controller.js"),
                     new JsFileWithCdn("~/js/components/box/slideit.directive.js"),

                        new JsFileWithCdn("~/js/components/box/box.routes.js"),




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
                        new JsFileWithCdn("~/js/components/library/classChoose.controller.js"),
                        new JsFileWithCdn("~/js/components/library/classChooseDialog.controller.js"),
                        new JsFileWithCdn("~/js/components/share/invite.controller.js"),
                        new JsFileWithCdn("~/js/components/share/share.service.js"),

                        new JsFileWithCdn("~/js/components/item/item.controller.js"),
                        new JsFileWithCdn("~/js/components/item/comments.controller.js"),
                        new JsFileWithCdn("~/js/components/item/item.service.js"),
                        new JsFileWithCdn("~/js/components/item/dropFileClass.directive.js"),

                        new JsFileWithCdn("~/js/components/quiz/quiz.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.challenge.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.score.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.service.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.routes.js"),

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
                        //new JsFileWithCdn("~/js/shared/sounds.js"),
                        //new JsFileWithCdn("~/js/shared/soundAlert.directive.js"),
                        new JsFileWithCdn("~/js/shared/notification.js"),
                        new JsFileWithCdn("~/js/shared/pagingOnScroll.directive.js"),
                        new JsFileWithCdn("~/js/shared/sbScroll.directive.js"),

                        new JsFileWithCdn("~/js/components/box/box.service.js"),
                        new JsFileWithCdn("~/js/components/uploadWrapper.controller.js"),
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
                        new JsFileWithCdn("~/js/shared/cartAnimation.directive.js"),



                        new JsFileWithCdn("~/js/components/chat/chat.controller.js"),
                        new JsFileWithCdn("~/js/components/chat/conversation.controller.js"),
                        new JsFileWithCdn("~/js/components/chat/chatUsers.controller.js"),

                        new JsFileWithCdn("~/js/components/chat/chat.factory.js"),
                        new JsFileWithCdn("~/js/components/chat/hubFactory.js"),
                        new JsFileWithCdn("~/js/components/chat/chatTimeAgo.js"),
                        new JsFileWithCdn("~/js/components/chat/previewController.js"),
                        new JsFileWithCdn("~/js/components/chat/toggleChat.directive.js"),
                        new JsFileWithCdn("~/js/components/chat/hideChatOnMobile.directive.js"),
                         new JsFileWithCdn("~/js/initAngular.js")
                    }

                }
            };



            return jsDictionary;
        }

    }
}