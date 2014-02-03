using System;
using System.Reflection;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Zbang.Zbox.Mvc3WebRole.App_Start.AzureCdn
{
    public static class CdnHelper
    {
        public static void Config()
        {
            var cdnEndpoint = "";
            cdnEndpoint = GetValueFromCloudConfig(cdnEndpoint);
            CdnHelpersContext.Current.Configure(c =>
            {
                c.CdnEndointUrl = cdnEndpoint;// "az32005.vo.msecnd.net";
                //c.EnableBlobStorageBacking(CloudStorageAccount.DevelopmentStorageAccount);
                //c.EnableImageOptimizations();
                c.UseCdnForContentFolder();
                c.UseCdnForScriptsFolder();
                c.HashKey = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                c.DebuggingEnabled = () => System.Web.HttpContext.Current.Request.IsLocal || string.IsNullOrWhiteSpace(cdnEndpoint);

            });

            CdnHelpersContext.Current.RegisterCombinedJsFiles("core",
                "~/Scripts/Ext/Modernizr.js",
                "~/Scripts/Ext/ZeroClipboard.js",
                "~/Scripts/Ext/DateFormat.js",
                "~/Scripts/Ext/jquery.validate.hooks.js",
                "~/Scripts/Ext/jquery.watermark.min.js",
                "~/Scripts/Ext/sSelect.js",

                "~/Scripts/Ext/ddSelect.js",
                "~/Scripts/Ext/elasticTextBox.js",
                "~/Scripts/Ext/checkBox.js",
                "~/Scripts/plupload/plupload.full.js",
                "~/Scripts/Ext/jquery.idle-time.js",

                "~/Scripts/Zbox/bootstrapper.js",
                "~/Scripts/Zbox/InviteViewModel.js",

                "~/Scripts/Zbox/Zbox.js",
                "~/Scripts/Zbox/Zbox.Security.js",
                "~/Scripts/Zbox/Zbox.Boxes.js",
                "~/Scripts/Zbox/Zbox.Logon.js",
                "~/Scripts/Zbox/ZboxAjaxRequest.js",
                "~/Scripts/Zbox/Zbox.Share.js",
                "~/Scripts/Zbox/Zbox.Resize.js",
                "~/Scripts/Zbox/ZboxAccountSettings.js",

                "~/Scripts/Zbox/Zbox.UploadFile.js",
                "~/Scripts/Ext/google-analytics.js",

                "~/Scripts/Zbox/BoxViewModel.js",
                "~/Scripts/Zbox/CommentsViewModel.js",
                "~/Scripts/Zbox/RecentActivityViewModel.js",
                "~/Scripts/Zbox/BoxItemsViewModel.js",
                "~/Scripts/Zbox/ItemViewModel.js",
                "~/Scripts/Zbox/BoxesViewModel.js",
                "~/Scripts/Zbox/SearchViewModel.js",
                "~/Scripts/Zbox/BoxSettingsViewModel.js"
            );
            CdnHelpersContext.Current.RegisterCombinedJsFiles("upload",
                "~/Scripts/Ext/Modernizr.js",
                "~/Scripts/Ext/ZeroClipboard.js",
                "~/Scripts/Ext/DateFormat.js",
                "~/Scripts/Ext/jquery.validate.hooks.js",
                "~/Scripts/Ext/jquery.watermark.min.js",
                "~/Scripts/Ext/sSelect.js",
                "~/Scripts/Ext/ddSelect.js",
                "~/Scripts/Ext/checkBox.js",
                "~/Scripts/plupload/plupload.full.js",

                "~/Scripts/Zbox/Zbox.js",
                "~/Scripts/Zbox/Zbox.Security.js",
                "~/Scripts/Zbox/Zbox.Boxes.js",
                "~/Scripts/Zbox/Zbox.Logon.js",
                "~/Scripts/Zbox/ZboxAjaxRequest.js",
                "~/Scripts/Zbox/Zbox.Share.js",
                "~/Scripts/Zbox/Zbox.Resize.js",
                "~/Scripts/Zbox/ZboxAccountSettings.js",

                "~/Scripts/Zbox/Zbox.UploadFile.js",
                "~/Scripts/Ext/google-analytics.js");

            CdnHelpersContext.Current.RegisterCombinedJsFiles("newCode",
                 "~/Scripts/Ext/Modernizr.js",
            "~/Scripts/Ext/DateFormat.js",
            "~/Scripts/Ext/jquery.watermark.min.js",
             "~/Scripts/Ext/jquery.ba-bbq.js",

             "~/Scripts/Zbox/bootstrapper.js",

            "~/Scripts/Zbox/Zbox.js",
            "~/Scripts/Zbox/Zbox.Security.js",
            "~/Scripts/Zbox/Zbox.Boxes.js",
            "~/Scripts/Zbox/ZboxAjaxRequest.js",

            "~/Scripts/Ext/google-analytics.js",
            "~/Scripts/Zbox/BoxesViewModel.js",
            "~/Scripts/Zbox/Zbox.Share.js",

            "~/Scripts/Zbox/TagsViewModel.js",
            "~/Scripts/Zbox/SelectedTagsViewModel.js",
            "~/Scripts/Zbox/FriendViewModel.js",
            "~/Scripts/Zbox/InlineSearchViewModel.js",
            "~/Scripts/Zbox/WallViewModel.js",
            "~/Scripts/Zbox/InviteViewModel.js"
                );

            CdnHelpersContext.Current.RegisterCombinedCssFiles("site",
                "~/Content/Site.css",
                "~/Content/Account/account.css",
                "~/Content/Box.css",
                "~/Content/comments.css",
                "~/Content/ActionData.css",
                "~/Content/Search.css",
                "~/Content/ListView.css",
                "~/Content/ThumbView.css"

            );
            CdnHelpersContext.Current.RegisterCombinedCssFiles("site100",
                "~/Content100/site100.css",
                "~/Content100/item100.css"

            );
            CdnHelpersContext.Current.RegisterCombinedCssFiles("newSite",
                "~/Content/New/Site.css", "~/Content/New/Boxes.css", "~/Content/New/Box.css"
            );
            CdnHelpersContext.Current.RegisterCombinedCssFiles("Upload",
                "~/Content/Upload.css"
            );
            CdnHelpersContext.Current.RegisterCombinedCssFiles("Rtl",
                "~/Content/rtl.css"
            );
            CdnHelpersContext.Current.RegisterCombinedCssFiles("Rtl100",
                "~/Content100/rtl100.css");
        }

        private static string GetValueFromCloudConfig(string cdnEndpoint)
        {
            try
            {
                cdnEndpoint = RoleEnvironment.GetConfigurationSettingValue("CdnEndpoint");
            }
            catch (Exception)
            {
            }
            return cdnEndpoint;
        }
    }
}