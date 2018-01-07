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
        public static RedirectResult RedirectToOldSite(this Controller conteroller)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = "heb.spitball.co",
                Path = conteroller.HttpContext.Request.Path,
                Query = conteroller.HttpContext.Request.QueryString.Value
            };
            return conteroller.Redirect(uriBuilder.ToString());
        }
    }
}
