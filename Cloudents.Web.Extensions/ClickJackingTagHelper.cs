using System;
using Microsoft.AspNetCore.Builder;

namespace Cloudents.Web.Extensions
{
    public static class ClickJackingTagHelper
    {
        public const string FullScript = "<style id=\"acj\">body{display:none !important;}</style>" +
                                         "<script type=\"text/javascript\">" +
                                         "if(self===top){var acj = document.getElementById(\"acj\"); acj.parentNode.removeChild(acj)" +
                                         "}else top.location=self.location;" +
                                         "</script> ";


        public static IApplicationBuilder UseClickJacking(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClickJackingMiddleware>(Array.Empty<object>());
        }
    }
}
