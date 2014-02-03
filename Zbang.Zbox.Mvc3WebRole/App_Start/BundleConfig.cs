using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.Mvc3WebRole.App_Start
{
    public static class BundleConfig
    {
        static Dictionary<string, string> cssBundels = new Dictionary<string, string>();
        static Dictionary<string, string> jsBundels = new Dictionary<string, string>();
        static string cdnLocation = GetValueFromCloudConfig();

        public static string CssLink(string key)
        {
            return cssBundels[key];
        }
        public static string JsLink(string key)
        {
            return jsBundels[key];
        }

        public static string CdnEndpointUrl
        {
            get
            {
                return cdnLocation;
            }
        }

        public static void RegisterBundle()
        {
            RegisterCss("newcore",
                "~/Content/Site.css",
                "~/Content/Boxes.css",
                "~/Content/Box.css",
                "~/Content/Search.css",
                "~/Content/Comments.css",
                "~/Content/Account.css",
                "~/Content/Tooltip.css",
                "~/Content/Item.css",
                "~/Content/Upload.css",
                "~/Content/Settings.css",
                "~/Content/Static.css");

            RegisterCss("newrtl", "~/Content/rtl.css");

            RegisterCss("newcore3", "~/Content/Site3.css");

            RegisterCss("newrtl3", "~/Content/rtl3.css");

            //old layout
            //RegisterCss("site", "~/Content/Site.css", "~/Content/Account/account.css", "~/Content/Box.css", "~/Content/Comments.css",
            //            "~/Content/ActionData.css", "~/Content/ListView.css", "~/Content/ThumbView.css", "~/Content/Search.css");
            //RegisterCss("upload", "~/Content/Site.css", "~/Content/Account/account.css", "~/Content/Box.css", "~/Content/Comments.css",
            //            "~/Content/ActionData.css", "~/Content/Search.css", "~/Content/ListView.css", "~/Content/ThumbView.css",
            //            "~/Content/Upload.css");
            //RegisterCss("rtl", "~/Content/rtl.css");

            //RegisterJs("jquery", new JsFileWithCdn("~/Scripts/jquery-1.7.2.min.js", "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.2.min.js"));

            //RegisterJs("core",
            //    new JsFileWithCdn("~/Scripts/jquery.validate.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.unobtrusive-ajax.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery-ui-1.8.23.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.23/jquery-ui.min.js"),
            //    new JsFileWithCdn("~/Scripts/knockout.js", "https://ajax.aspnetcdn.com/ajax/knockout/knockout-2.1.0.js"),
            //    new JsFileWithCdn("~/Scripts/Modernizr.js"),
            //    new JsFileWithCdn("~/Scripts/ZeroClipboard.js"),
            //    new JsFileWithCdn("~/Scripts/DateFormat.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.validate.hooks.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.watermark.min.js"),
            //    new JsFileWithCdn("~/Scripts/sSelect.js"),
            //    new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
            //    //new JsFileWithCdn("~/Scripts/json2.min.js"),
            //    new JsFileWithCdn("~/Scripts/ddSelect.js"),
            //    new JsFileWithCdn("~/Scripts/elasticTextBox.js"),
            //    new JsFileWithCdn("~/Scripts/checkBox.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.full.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.idle-time.js"),
            //    new JsFileWithCdn("~/Js/bootstrapper.js"),
            //    new JsFileWithCdn("~/Js/InviteViewModel.js"),
            //    new JsFileWithCdn("~/Js/Zbox.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Security.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Boxes.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Logon.js"),
            //    new JsFileWithCdn("~/Js/ZboxAjaxRequest.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Share.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Resize.js"),
            //    new JsFileWithCdn("~/Js/ZboxAccountSettings.js"),
            //   // new JsFileWithCdn("~/Js/Zbox.UploadFile.js"),

            //    new JsFileWithCdn("~/Js/BoxViewModel.js"),
            //    new JsFileWithCdn("~/Js/CommentsViewModel.js"),
            //    //new JsFileWithCdn("~/Js/RecentActivityViewModel.js"),
            //    new JsFileWithCdn("~/Js/BoxItemsViewModel.js"),
            //    //new JsFileWithCdn("~/Js/ItemViewModel.js"),
            //    new JsFileWithCdn("~/Js/BoxesViewModel.js"),
            //    new JsFileWithCdn("~/Js/SearchViewModel.js"),
            //    new JsFileWithCdn("~/Js/BoxSettingsViewModel.js"));

            //RegisterJs("upload",
            //    new JsFileWithCdn("~/Scripts/jquery.validate.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.unobtrusive-ajax.min.js"),
            //    new JsFileWithCdn("~/Scripts/jquery-ui-1.8.23.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.23/jquery-ui.min.js"),
            //    new JsFileWithCdn("~/Scripts/Modernizr.js"),
            //    new JsFileWithCdn("~/Scripts/ZeroClipboard.js"),
            //    new JsFileWithCdn("~/Scripts/DateFormat.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.validate.hooks.js"),
            //    new JsFileWithCdn("~/Scripts/jquery.watermark.min.js"),
            //    new JsFileWithCdn("~/Scripts/sSelect.js"),
            //    new JsFileWithCdn("~/Scripts/ddSelect.js"),
            //    new JsFileWithCdn("~/Scripts/checkBox.js"),
            //    new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
            //    //new JsFileWithCdn("~/Scripts/json2.min.js"),
            //    new JsFileWithCdn("~/Scripts/plupload/plupload.full.js"),
            //    new JsFileWithCdn("~/Js/Zbox.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Security.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Boxes.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Logon.js"),
            //    new JsFileWithCdn("~/Js/ZboxAjaxRequest.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Share.js"),
            //    new JsFileWithCdn("~/Js/Zbox.Resize.js"),
            //    new JsFileWithCdn("~/Js/ZboxAccountSettings.js")
            //    //new JsFileWithCdn("~/Js/Zbox.UploadFile.js")
            //    );

            RegisterJs("newCode",
                new JsFileWithCdn("~/Scripts/jquery-1.8.3.min.js", "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.unobtrusive-ajax.min.js"),
                new JsFileWithCdn("~/Scripts/jquery-ui-1.8.23.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.23/jquery-ui.min.js"),
                new JsFileWithCdn("~/Scripts/knockout-2.2.0.js", "https://ajax.aspnetcdn.com/ajax/knockout/knockout-2.2.0.js"),
                //new JsFileWithCdn("~/Scripts/json2.min.js"),
                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                new JsFileWithCdn("~/Scripts/Modernizr.js"),
                new JsFileWithCdn("~/Scripts/ZeroClipboard.js"),
                new JsFileWithCdn("~/Scripts/plupload/plupload.full.js"),
                //new JsFileWithCdn("~/Scripts/DateFormat.js"),
                //new JsFileWithCdn("~/Scripts/jquery.watermark.min.js"),// we dont need that anymore
                //new JsFileWithCdn("~/Scripts/checkBox.js"),
                new JsFileWithCdn("~/Scripts/elasticTextBox.js"),
                //new JsFileWithCdn("~/Scripts/jquery.ba-bbq.js"),
                new JsFileWithCdn("~/Scripts/history.js/history.js"),
                new JsFileWithCdn("~/Scripts/history.js/history.adapter.jquery.js"),
                new JsFileWithCdn("~/Scripts/history.js/history.html4.js"),
                
                //new JsFileWithCdn("~/Scripts/jquery.idle-time.js"),

                new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
                new JsFileWithCdn("~/Js/Dialog.js"), //dialog message
                new JsFileWithCdn("~/Js/AjaxWrapper.js"), // ajax wrapper
                new JsFileWithCdn("~/Js/bootstrapper.js"),
                new JsFileWithCdn("~/Js/bootstrapper2.js"),
                
                 //updadtes
                 new JsFileWithCdn("~/Js/Updates.js"),

                new JsFileWithCdn("~/Js/Zbox.js"), // remove that
                new JsFileWithCdn("~/Js/Zbox.Security.js"), //remove that
                new JsFileWithCdn("~/Js/Zbox.Boxes.js"), //legacy for wall view model
                new JsFileWithCdn("~/Js/ZboxAjaxRequest.js"), // check that
                //new JsFileWithCdn("~/Js/Zbox.Share.js"), // we need to change that

                //header
                new JsFileWithCdn("~/Js/InviteViewModel.js"),

                //dashboard page

                //new JsFileWithCdn("~/Js/TagsViewModel.js"),
                //new JsFileWithCdn("~/Js/SelectedTagsViewModel.js"),
                //new JsFileWithCdn("~/Js/FriendViewModel.js"),
                //new JsFileWithCdn("~/Js/InlineSearchViewModel.js"),
                //new JsFileWithCdn("~/Js/WallViewModel.js"),

                new JsFileWithCdn("~/Js/BoxesViewModel.js"),
                new JsFileWithCdn("~/Js/Library.js"),
                //new JsFileWithCdn("~/Js/Dashboard.js"),


                //box page
                new JsFileWithCdn("~/Js/BoxItemsViewModel.js"),
                new JsFileWithCdn("~/Js/BoxViewModel.js"),
                new JsFileWithCdn("~/Js/CommentsViewModel.js"),
                //new JsFileWithCdn("~/Js/UploadLinkBox.js"),

                //Search
                 new JsFileWithCdn("~/Js/SearchViewModel.js"),

                 //account
                 new JsFileWithCdn("~/Js/Zbox.Logon.js"), //we need to fix that

                 //item page
                 new JsFileWithCdn("~/Js/ItemViewModel2.js"),

                 //upload
                 new JsFileWithCdn("~/Js/Upload.js"),

                 //invite/Share/message
                 new JsFileWithCdn("~/Js/InviteShare.js"),



                 //box settings
                 new JsFileWithCdn("~/Js/BoxSettings.js"),

                 //account settings
                 new JsFileWithCdn("~/Js/AccountSettings.js")


                );
            RegisterJs("cd1",
                new JsFileWithCdn("~/Scripts/jquery-1.8.3.min.js", "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.validate.unobtrusive.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.min.js"),
                new JsFileWithCdn("~/Scripts/jquery.unobtrusive-ajax.min.js", "https://ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.unobtrusive-ajax.min.js"),
                //new JsFileWithCdn("~/Scripts/jquery-ui-1.8.23.min.js", "https://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.23/jquery-ui.min.js"),
                new JsFileWithCdn("~/Scripts/knockout-2.2.0.js", "https://ajax.aspnetcdn.com/ajax/knockout/knockout-2.2.0.js"),
                //new JsFileWithCdn("~/Scripts/json2.min.js"),
                new JsFileWithCdn("~/Scripts/externalScriptLoader.js"),
                new JsFileWithCdn("~/Scripts/Modernizr.js"),
                //new JsFileWithCdn("~/Scripts/ZeroClipboard.js"),
                //new JsFileWithCdn("~/Scripts/plupload/plupload.full.js"),
                //new JsFileWithCdn("~/Scripts/DateFormat.js"),
                //new JsFileWithCdn("~/Scripts/jquery.watermark.min.js"),// we dont need that anymore
                //new JsFileWithCdn("~/Scripts/checkBox.js"),
                //new JsFileWithCdn("~/Scripts/elasticTextBox.js"),
                //new JsFileWithCdn("~/Scripts/jquery.ba-bbq.js"),
                new JsFileWithCdn("~/Scripts/history.js/history.js"),
                new JsFileWithCdn("~/Scripts/history.js/history.adapter.jquery.js"),
                new JsFileWithCdn("~/Scripts/history.js/history.html4.js"),
                //new JsFileWithCdn("~/Scripts/jquery.idle-time.js"),

                new JsFileWithCdn("~/Js/Bindings.js"), //knockout new bindings
                new JsFileWithCdn("~/Js/Dialog.js"), //dialog message
                new JsFileWithCdn("~/Js/AjaxWrapper.js"), // ajax wrapper
                new JsFileWithCdn("~/Js/bootstrapper.js"),
                new JsFileWithCdn("~/Js/bootstrapper2.js"),
                new JsFileWithCdn("~/Js/pubsub.js"),

                 //updadtes
                 new JsFileWithCdn("~/Js/Updates.js"),

                //new JsFileWithCdn("~/Js/Zbox.js"), // remove that
                //new JsFileWithCdn("~/Js/Zbox.Security.js"), //remove that
                //new JsFileWithCdn("~/Js/Zbox.Boxes.js"), //legacy for wall view model
                //new JsFileWithCdn("~/Js/ZboxAjaxRequest.js"), // check that
                //new JsFileWithCdn("~/Js/Zbox.Share.js"), // we need to change that

                //header
                new JsFileWithCdn("~/Js/InviteViewModel2.js"),
                

                //dashboard page

                //new JsFileWithCdn("~/Js/TagsViewModel.js"),
                //new JsFileWithCdn("~/Js/SelectedTagsViewModel.js"),
                //new JsFileWithCdn("~/Js/FriendViewModel.js"),
                //new JsFileWithCdn("~/Js/InlineSearchViewModel.js"),
                //new JsFileWithCdn("~/Js/WallViewModel.js"),

                new JsFileWithCdn("~/Js/BoxesViewModel.js"),
                new JsFileWithCdn("~/Js/Library.js")
                //new JsFileWithCdn("~/Js/Dashboard.js"),


                //box page
                //new JsFileWithCdn("~/Js/BoxItemsViewModel.js"),
                //new JsFileWithCdn("~/Js/BoxViewModel.js"),
                //new JsFileWithCdn("~/Js/CommentsViewModel.js"),
                //new JsFileWithCdn("~/Js/UploadLinkBox.js"),

                //Search
                 //new JsFileWithCdn("~/Js/SearchViewModel.js"),

                 ////account
                 //new JsFileWithCdn("~/Js/Zbox.Logon.js"), //we need to fix that

                 ////item page
                 //new JsFileWithCdn("~/Js/ItemViewModel2.js"),

                 ////upload
                 //new JsFileWithCdn("~/Js/Upload.js"),

                 ////invite/Share/message
                 //new JsFileWithCdn("~/Js/InviteShare.js"),



                 ////box settings
                 //new JsFileWithCdn("~/Js/BoxSettings.js"),

                 ////account settings
                 //new JsFileWithCdn("~/Js/AccountSettings.js")
                );

            CopyFilesToCdn("Content", "*.png");
            CopyFilesToCdn("Content", "*.jpg");
            CopyFilesToCdn("Content", "*.gif");

        }

        public static bool IsDebugEnabled()
        {
            return System.Web.HttpContext.Current.Request.IsLocal || string.IsNullOrWhiteSpace(cdnLocation);
        }

        private static void RegisterCss(string key, params string[] cssFiles)
        {
            var cssbundle = SquishIt.Framework.Bundle.Css();
            foreach (var cssFile in cssFiles)
            {
                cssbundle.Add(cssFile);
            }
            var cdnUrl = cdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                cssbundle.WithOutputBaseHref(cdnUrl);
                cssBundels.Add(key, cssbundle.Render("~/css_#.css"));
            }
            else
            {
                cssBundels.Add(key, cssbundle.Render("~/cdn/css_#.css"));
            }

        }

        private static void RegisterJs(string key, params JsFileWithCdn[] jsFiles)
        {
            var jsBundle = SquishIt.Framework.Bundle.JavaScript();
            foreach (var jsFile in jsFiles)
            {
                if (string.IsNullOrWhiteSpace(jsFile.CdnFile))
                {
                    jsBundle.Add(jsFile.LocalFile);
                }
                else
                {
                    jsBundle.AddRemote(jsFile.LocalFile, jsFile.CdnFile);
                }
            }
            var cdnUrl = cdnLocation;
            if (!string.IsNullOrWhiteSpace(cdnUrl))
            {
                jsBundle.WithOutputBaseHref(cdnUrl);
                jsBundels.Add(key, jsBundle.Render("~/js_#.js"));
            }
            else
            {
                jsBundels.Add(key, jsBundle.Render("~/cdn/js_#.js"));
            }
        }

        private static string GetValueFromCloudConfig()
        {
            try
            {
                return RoleEnvironment.GetConfigurationSettingValue("CdnEndpoint");
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        private static void CopyFilesToCdn(string directoryRelativePath, string fileSearchOption)
        {
            var server = HttpContext.Current.Server;
            var appRoot = server.MapPath("~/");
            var cdnRoot = server.MapPath("~/cdn");

            var filesPath = System.IO.Directory.GetFiles(server.MapPath(directoryRelativePath), fileSearchOption, System.IO.SearchOption.AllDirectories);

            foreach (var filePath in filesPath)
            {
                var relativePath = filePath.Replace(appRoot, string.Empty);
                var cdnFilePath = Path.Combine(cdnRoot, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(cdnFilePath));
                File.Copy(filePath, Path.Combine(cdnRoot, relativePath), true);
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