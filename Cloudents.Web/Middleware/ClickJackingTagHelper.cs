using Microsoft.AspNetCore.Builder;
using System;

namespace Cloudents.Web.Middleware
{
    public static class ClickJackingTagHelper
    {
        public const string FullScript = "<style id=\"acj\">body{display:none !important;}</style>" +
                                         "<script defer type=\"text/javascript\">" +
                                         "if(self===top){var acj = document.getElementById(\"acj\"); acj.parentNode.removeChild(acj)" +
                                         "}else top.location=self.location;" +
                                         "</script> ";


        public static IApplicationBuilder UseClickJacking(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClickJackingMiddleware>(Array.Empty<object>());
        }
    }
}
