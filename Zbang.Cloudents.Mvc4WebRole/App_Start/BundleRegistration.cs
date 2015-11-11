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
                        "~/bower_components/angular-material/material.css",
                        "~/bower_components/textAngular/dist/textAngular.css",
                        
                        "~/Content/signin/font-awesome.css",
                        //"~/Content/signin/simple-line-icons.css",
                        "~/Content/homepage/bootstrap.css",
                        "~/Content/signin/uniform.default.css",
                        "~/Content/site/bootstrap-switch.css",
                        "~/Content/site/portfolio.css",
                        "~/content/site/profile.css",
                        "~/Content/site/components-rounded.css",
                        "~/Content/site/plugins.css",
                        "~/Content/site/layout.css",
                        "~/Content/site/light.css",
                        
                        "~/Content/site/menu.css",

                        "~/content/site/morphsearch.css",
                        
                        "~/Content/site/header.css",
                        "~/content/site/general.css",
                        "~/Content/site/dashboard.css",
                        "~/Content/site/box.css",
                        "~/Content/site/user.css",
                        "~/Content/site/accountSettings.css",
                        "~/Content/site/library.css",
                        "~/Content/site/item.css",
                        "~/Content/site/quiz.css",
                         "~/Content/site/quizCreate.css",
                        "~/Content/site/share.css",
                        "~/content/site/leaderboard.css",
                        "~/content/site/search.css"
                       
                    }
                },
            };
            return cssDictionary;
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<JsFileWithCdn>>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>
            {
                
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
                    "homePage", new[]
                    {
                        new JsFileWithCdn("~/js/homePage/theme.js"),
                        //new JsFileWithCdn("~/Scripts/browser-deeplink.min.js")
                        new JsFileWithCdn("~/js/homePage/homeScreen.js"),
                        new JsFileWithCdn("~/js/signin/login-soft.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js")

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
                        new JsFileWithCdn("~/js/signin/login-soft.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js")
                    }
                },
                {
                    "site4", new[]
                    {

                        new JsFileWithCdn("~/js/signin/jquery.min.js"),
                        new JsFileWithCdn("~/js/signin/jquery-migrate.min.js"),
                        new JsFileWithCdn("~/scripts/Modernizr.js"),
                        
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
                        //new JsFileWithCdn("~/scripts/angular-sanitize.js"
                        //    //"https://ajax.googleapis.com/ajax/libs/angularjs/1.4.4/angular-sanitize.min.js"
                        //    ),
                        new JsFileWithCdn("~/scripts/angular-ui-router.js"),
                        new JsFileWithCdn("~/scripts/angulartics.js"),
                        new JsFileWithCdn("~/scripts/angulartics-ga.js"),
                        new JsFileWithCdn("~/scripts/angular-cache-2.4.1.js"),
                        new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular-rangy.min.js"),
                        new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular-sanitize.min.js"),
                        

                        new JsFileWithCdn("~/bower_components/textAngular/dist/textAngularSetup.js"),
                        //new JsFileWithCdn("~/scripts/textAngular/textAngularSetup.js"),
                        new JsFileWithCdn("~/bower_components/textAngular/dist/textAngular.js"),

                        
                        new JsFileWithCdn("~/bower_components/angular-animate/angular-animate.js"),
                        new JsFileWithCdn("~/bower_components/angular-aria/angular-aria.js"),
                        new JsFileWithCdn("~/bower_components/angular-material/angular-material.js"),

                        new JsFileWithCdn("~/scripts/ng-infinite-scroll.js"),

                        new JsFileWithCdn("~/scripts/plupload2/moxie.min.js"),
                        new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),
                        new JsFileWithCdn("~/scripts/plupload2/angular-plupload2.js"),

                        new JsFileWithCdn("~/scripts/ui-bootstrap-custom-tpls-0.14.1.min.js"),
                        new JsFileWithCdn("~/scripts/site/bootstrap-tabdrop.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js"),
                        new JsFileWithCdn("~/js/polyfills.js"),

                        new JsFileWithCdn("~/js/signin/metronic.js"),
                        new JsFileWithCdn("~/js/signin/layout.js"),
                        new JsFileWithCdn("~/js/signin/demo.js"),

                        new JsFileWithCdn("~/js/app.js"),
                        new JsFileWithCdn("~/js/app.config.js"),
                        new JsFileWithCdn("~/js/app.route.js"),
                        new JsFileWithCdn("~/js/routerHelperProvider.js"),

                        new JsFileWithCdn("~/js/components/quiz/stopwatch.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.filter.js"),
                        new JsFileWithCdn("~/js/components/quiz/stopwatch.directive.js"),


                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.challenge.module.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.score.module.js"),
                        new JsFileWithCdn("~/js/components/userdetails/userdetails.module.js"),
                        new JsFileWithCdn("~/js/components/user/userdetails.controller.js"),
                        new JsFileWithCdn("~/js/components/user/user.controller.js"),
                        
                        new JsFileWithCdn("~/js/components/user/user.service.js"),
                        new JsFileWithCdn("~/js/components/user/updates.service.js"),

                        new JsFileWithCdn("~/js/components/dashboard/dashboard.service.js"),
                        new JsFileWithCdn("~/js/components/dashboard/dashboard.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/sidemenu.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/university.controller.js"),
                        //new JsFileWithCdn("~/js/components/dashboard/createClass.controller.js"),
                        new JsFileWithCdn("~/js/components/dashboard/createBox.controller.js"),

                        new JsFileWithCdn("~/js/components/box/box.controller.js"),
                        new JsFileWithCdn("~/js/components/box/box.routes.js"),
                        new JsFileWithCdn("~/js/components/box/feed.controller.js"),
                        new JsFileWithCdn("~/js/components/box/item.controller.js"),
                        new JsFileWithCdn("~/js/components/box/quizzes.controller.js"),
                        new JsFileWithCdn("~/js/components/box/members.controller.js"),
                        new JsFileWithCdn("~/js/components/box/box.service.js"),
                        new JsFileWithCdn("~/js/components/box/recommended.controller.js"),

                        new JsFileWithCdn("~/js/components/account/account.controller.js"),
                        
                        new JsFileWithCdn("~/js/components/account/notification.controller.js"),
                        new JsFileWithCdn("~/js/components/account/password.controller.js"),
                        new JsFileWithCdn("~/js/components/account/info.controller.js"),
                        new JsFileWithCdn("~/js/components/account/account.service.js"),
                        new JsFileWithCdn("~/js/components/account/account.routes.js"),

                       


                        new JsFileWithCdn("~/js/components/search.controller.js"),
                        new JsFileWithCdn("~/js/components/leaderboard.controller.js"),

                        new JsFileWithCdn("~/js/components/library/library.controller.js"),

                        new JsFileWithCdn("~/js/components/share/invite.controller.js"),
                        new JsFileWithCdn("~/js/components/share/share.service.js"),
                        

                        new JsFileWithCdn("~/js/components/item/item.controller.js"),
                        new JsFileWithCdn("~/js/components/item/item.service.js"),

                        new JsFileWithCdn("~/js/components/quiz/quiz.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.challenge.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/popup/quiz.score.controller.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.service.js"),
                        new JsFileWithCdn("~/js/components/quiz/quiz.routes.js"),
                        new JsFileWithCdn("~/js/components/quiz/quizCreate.controller.js"),


                        new JsFileWithCdn("~/js/components/app.controller.js"),
                        new JsFileWithCdn("~/js/services/ajaxService.js"),

                        new JsFileWithCdn("~/js/shared/colorOnLength.js"),
                        new JsFileWithCdn("~/js/shared/loader.js"),
                        new JsFileWithCdn("~/js/shared/userimage.js"),
                        new JsFileWithCdn("~/js/shared/megaNumbers.js"),
                        new JsFileWithCdn("~/js/shared/focusMe.js"),
                        new JsFileWithCdn("~/js/shared/displayTime.js"),
                        new JsFileWithCdn("~/js/shared/userDetails.js"),
                        new JsFileWithCdn("~/js/shared/tabDrop.js"),
                        new JsFileWithCdn("~/js/shared/showForm.js"),
                        new JsFileWithCdn("~/js/shared/fileReader.js"),
                        new JsFileWithCdn("~/js/shared/animClass.js"),

                        new JsFileWithCdn("~/js/components/item/upload.controller.js"),
                        new JsFileWithCdn("~/js/components/item/externalProviderUpload.service.js"),
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