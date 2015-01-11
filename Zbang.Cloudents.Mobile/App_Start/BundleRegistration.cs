using System.Collections.Generic;
using Zbang.Cloudents.Mvc4WebRole;

namespace Zbang.Cloudents.Mobile
{
    public static class BundleRegistration
    {
        public static void RegisterBundles()
        {
            BundleConfig.RegisterBundle(RegisterCss(), RegisterJs());

        }

        private static IDictionary<string, IEnumerable<string>> RegisterCss()
        {
            var cssDictionary = new Dictionary<string, IEnumerable<string>>
            {
                {"lang.ru-RU", new[] {"~/Content/lang.ru-RU.css"}},
                {"lang.he-IL", new[] {"~/Content/lang.he-IL.css"}},

            };

            cssDictionary.Add("mobile", new[] {
                "~/Content/Normalize.css",
                "~/Content/Animations.css",
                "~/Content/Mobile.css",
                "~/Content/Animations.css",
                "~/Content/Registration.css",
                "~/Content/SVG.css",
                "~/Content/Home.css",
                "~/Content/ChooseSchool.css",
                "~/Content/Dashboard.css",
                "~/Content/Search.css",
                "~/Content/Box.css",
                "~/Content/Feed.css"});
            cssDictionary.Add("mobileRtl", new[] { 
                "~/Content/MobileRtl.css" ,
                "~/Content/RegistrationRtl.css",
                "~/Content/HomeRtl.css",
                "~/Content/ChooseSchoolRtl.css",
                "~/Content/DashboardRtl.css",
                "~/Content/SearchRtl.css",
                "~/Content/BoxRtl.css",
                "~/Content/FeedRtl.css"});

            cssDictionary.Add("itemMobile", new[] { "~/Content/ItemMobile.css" });
            cssDictionary.Add("itemMobileRtl", new[] { "~/Content/ItemMobileRtl.css" });

            return cssDictionary;
        }

        private static IDictionary<string, IEnumerable<JsFileWithCdn>> RegisterJs()
        {
            var jsDictionary = new Dictionary<string, IEnumerable<JsFileWithCdn>>
            {
             {
                    "angular", new[]
                    {
                        //new JsFileWithCdn("~/scripts/jquery.min.js",
                        //"https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),                        
                        new JsFileWithCdn("~/scripts/angular.min.js",
                        "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular.min.js"),
                        new JsFileWithCdn("~/scripts/angular-messages.js"),
                        new JsFileWithCdn("~/scripts/angular-ui-router.js"),
                        new JsFileWithCdn("~/scripts/angular-touch.js"),
                        new JsFileWithCdn("~/scripts/angular-sanitize.js"),
                        new JsFileWithCdn("~/scripts/angular-animate.js",
                        "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.4/angular-animate.min.js"),
                        new JsFileWithCdn("~/scripts/angulartics.js"),
                        new JsFileWithCdn("~/scripts/angulartics-ga.js"),                        
                        new JsFileWithCdn("~/scripts/angular-cache-2.4.1.js"),
                        new JsFileWithCdn("~/scripts/stacktrace.js"),                                                                                                                                                
                        new JsFileWithCdn("~/scripts/plupload2/moxie.js"),
                        new JsFileWithCdn("~/scripts/plupload2/plupload.dev.js"),
                        new JsFileWithCdn("~/scripts/plupload2/angular-plupload.js"),
                        new JsFileWithCdn("~/scripts/svg4everybody.js"),

                        new JsFileWithCdn("~/js/app.module.js"),
                        new JsFileWithCdn("~/js/app.route.js"),

                        new JsFileWithCdn("~/js/components/account/accountController.js"),
                        new JsFileWithCdn("~/js/components/account/accountService.js"),

                        new JsFileWithCdn("~/js/components/register/registerController.js"),
                        new JsFileWithCdn("~/js/components/register/registerService.js"),
                        
                        new JsFileWithCdn("~/js/components/login/loginController.js"),
                        new JsFileWithCdn("~/js/components/login/loginService.js"),

                        new JsFileWithCdn("~/js/components/box/boxController.js"),
                        new JsFileWithCdn("~/js/components/box/boxService.js"),
                        new JsFileWithCdn("~/js/components/box/feed/feedController.js"),
                        new JsFileWithCdn("~/js/components/box/feed/feedService.js"),
                        new JsFileWithCdn("~/js/components/box/items/itemsController.js"),
                        new JsFileWithCdn("~/js/components/box/items/itemsService.js"),

                        new JsFileWithCdn("~/js/components/dashboard/dashboardController.js"),
                        new JsFileWithCdn("~/js/components/dashboard/dashboardService.js"),
                        new JsFileWithCdn("~/js/components/dashboard/menu/close.js"),

                        new JsFileWithCdn("~/js/components/libChoose/libChooseController.js"),
                        new JsFileWithCdn("~/js/components/libChoose/libChooseService.js"),

                        new JsFileWithCdn("~/js/components/search/searchController.js"),
                        new JsFileWithCdn("~/js/components/search/searchService.js"),

                        
                        new JsFileWithCdn("~/js/shared/ajax/ajaxService.js"),
                        new JsFileWithCdn("~/js/shared/ajax/account.js"),
                        new JsFileWithCdn("~/js/shared/ajax/library.js"),
                        new JsFileWithCdn("~/js/shared/ajax/dashboard.js"),
                        new JsFileWithCdn("~/js/shared/ajax/search.js"),
                        new JsFileWithCdn("~/js/shared/ajax/feed.js"),
                        new JsFileWithCdn("~/js/shared/ajax/box.js"),

                        new JsFileWithCdn("~/js/shared/loader/loader.js"),
                        new JsFileWithCdn("~/js/shared/loader/innerLoader.js"),

                        new JsFileWithCdn("~/js/shared/events/events.js"),
                        new JsFileWithCdn("~/js/shared/scroll/scroll.js"),
                        

                        new JsFileWithCdn("~/js/shared/userDetails.js"),

                        new JsFileWithCdn("~/js/libs/hammer.js"),
                        new JsFileWithCdn("~/js/libs/gestures.js"),
                        new JsFileWithCdn("~/js/libs/plupload.js"),
                        new JsFileWithCdn("~/js/libs/msdelastic.js"),
                        new JsFileWithCdn("~/js/libs/stacktrace.js"),
                        new JsFileWithCdn("~/js/libs/displayTime.js"),
                        new JsFileWithCdn("~/js/libs/textDirection.js"),
                        new JsFileWithCdn("~/js/libs/angular-facebook.js")


                        //new J
                    }
                    }
             };

            //{
            //    "home", new[]
            //    {
            //        new JsFileWithCdn("~/Scripts/validatinator.min.js"),
            //        new JsFileWithCdn("~/Js/staticShim.js"),
            //        new JsFileWithCdn("~/Js/Logon.js"),
            //        new JsFileWithCdn("~/Js/HomePage.js")
            //    }
            //},
            //{
            //    "General", new[]
            //    {
            //        new JsFileWithCdn("~/scripts/jquery.min.js",
            //            "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
            //        new JsFileWithCdn("~/scripts/jquery.validate.min.js"),
            //        new JsFileWithCdn("~/scripts/jquery.validate.unobtrusive.js"),// the script is too small
            //        new JsFileWithCdn("~/scripts/jquery.unobtrusive-ajax.js"), // the script is too small
            //        new JsFileWithCdn("~/scripts/Modernizr.js"),


            //        new JsFileWithCdn("~/Js/Utils2.js"),
            //        new JsFileWithCdn("~/scripts/externalScriptLoader.js"),

            //        new JsFileWithCdn("~/Js/pubsub2.js"),
            //        new JsFileWithCdn("~/scripts/svg4everybody.js")
            //    }
            //}
            //};
            //jsDictionary.Add("homeMobile", new[] {
            //  new JsFileWithCdn("~/Js/Mobile/Logon.js"),
            //  new JsFileWithCdn("~/Js/Mobile/Welcome.js")
            //});


            //jsDictionary.Add("mobile", new[] {
            //      new JsFileWithCdn("~/Scripts/jquery.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.validate.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.js"),


            //    new JsFileWithCdn("~/Scripts/knockout-3.0.0.js"),
            //    new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
            //    new JsFileWithCdn("~/Scripts/Modernizr.js"),

            //     new JsFileWithCdn("~/Scripts/plupload/plupload.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.html4.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.html5.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.flash.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.silverlight.js"),
            //    new JsFileWithCdn("~/Scripts/jquery-ui-1.10.4.min.js"),
            //    new JsFileWithCdn("~/Scripts/elasticTextBox.js"),

            //    new JsFileWithCdn("~/Js/Utils2.js"),
            //    new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
            //     new JsFileWithCdn("~/Js/pubsub2.js"),
            //    new JsFileWithCdn("~/Js/Cache2.js"),
            //    new JsFileWithCdn("~/Js/DataContext2.js"),

            //    new JsFileWithCdn("~/Js/Navigation2.js"),
            //    new JsFileWithCdn("~/Js/Mobile/MBaseViewModel.js"),
            //    new JsFileWithCdn("~/Js/Mobile/MInvite.js"),
            //    new JsFileWithCdn("~/Js/Mobile/MUpload.js"),
            //    //invite
            //    new JsFileWithCdn("~/Js/Mobile/MInviteViewModel.js"),
            //    //wall
            //    //new JsFileWithCdn("~/Js/Mobile/MWallViewModel.js"),
            //    //library
            //    new JsFileWithCdn("~/Js/Mobile/MLibrary.js"),
            //    //new JsFileWithCdn("/Js/Mobile/MLibraryChoose.js"),
            //    //dashboard
            //    new JsFileWithCdn("~/Js/Mobile/MBoxesViewModel.js"),

            //    //box
            //    new JsFileWithCdn("~/Js/Mobile/MBoxViewModel.js"),
            //    new JsFileWithCdn("~/Js/Mobile/MBoxItemsViewModel.js"),
            //    // new JsFileWithCdn("~/Js/Mobile/MCommentsViewModel.js"),

            //    //Account settings
            //     new JsFileWithCdn("~/Js/Mobile/MAccountSettings.js")
            //    });


            //jsDictionary.Add("homeMobile",new[] {
            //    new JsFileWithCdn("~/Js/Mobile/Logon.js"),
            //    new JsFileWithCdn("~/Js/Mobile/Welcome.js")
            //    });

            //jsDictionary.Add("MChooseLib",new[] {
            //    new JsFileWithCdn("~/Js/Mobile/MLibraryChoose.js")
            //    });


            jsDictionary.Add("mobileItem", new[] { new JsFileWithCdn("~/Scripts/jquery.min.js", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"),
                //"//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.2.min.js"),
                                    new JsFileWithCdn("~/Js/Utils2.js"),
                                    new JsFileWithCdn("~/Js/pubsub2.js"),
                                    new JsFileWithCdn("~/Js/Cache2.js"),
                                    new JsFileWithCdn("~/Js/DataContext2.js"),
                                    new JsFileWithCdn("~/Js/Mobile/MItemViewModel.js")
            });

            return jsDictionary;
        }

    }
}