using System;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static RedirectResult RedirectToOldSite(this Controller controller)
        {
            var path = controller.HttpContext.Request.Path.Value.TrimEnd('/');
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = "heb.spitball.co",
                Path = path + "/",
                Query = controller.HttpContext.Request.QueryString.Value
            };
            return controller.RedirectPermanent(uriBuilder.ToString());
        }
    }
}
