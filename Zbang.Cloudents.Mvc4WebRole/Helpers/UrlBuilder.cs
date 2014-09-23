using System;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class UrlBuilder
    {
        private readonly UrlHelper m_UrlHelper;
        public UrlBuilder(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            m_UrlHelper = new UrlHelper(httpContext.Request.RequestContext);

        }
        public static string NameToQueryString(string name)
        {
            return UrlConsts.NameToQueryString(name);
        }


        public string BuildDownloadUrl(long boxId, long itemId)
        {
            return m_UrlHelper.RouteUrl("ItemDownload", new
            {
                BoxUid = boxId, itemId
            });
        }
    }
}