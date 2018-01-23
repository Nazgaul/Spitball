using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static RedirectResult RedirectToOldSite(this Controller controller)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = "heb.spitball.co",
                Path = controller.HttpContext.Request.Path,
                Query = controller.HttpContext.Request.QueryString.Value
            };
            return controller.Redirect(uriBuilder.ToString());
        }
    }
}
