using System.Web;
using System.Web.Mvc;

namespace Zbang.Zbox.Mvc3WebRole.App_Start.AzureCdn
{
    public static class CdnUrlExtension
    {
        public static string CdnContent(this UrlHelper urlHelper, string contentPath)
        {
            if (BundleConfig.IsDebugEnabled())
            {
                return urlHelper.Content(contentPath);
            }
            var contentTypeAbsolutePath = VirtualPathUtility.ToAbsolute(contentPath);
            if (contentTypeAbsolutePath[0] == '/')
            {
                contentTypeAbsolutePath = contentTypeAbsolutePath.Remove(0, 1);
            }
            var cdnurl = VirtualPathUtility.AppendTrailingSlash(BundleConfig.CdnEndpointUrl) + contentTypeAbsolutePath;
            return urlHelper.Content(cdnurl);
        }
    }
}