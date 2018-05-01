using System;
using System.Net;
using Microsoft.AspNetCore.Rewrite;

namespace Cloudents.Web
{
    public class RemoveTrailingSlash : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            if (!string.Equals(context.HttpContext.Request.Method, "Get", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var request = context.HttpContext.Request;
            if (request.Path.Value == "/" || !request.Path.Value.EndsWith("/")) return;
            var uri = new UriBuilder
            {
                Query = context.HttpContext.Request.QueryString.Value.TrimStart('?'),
                Host = context.HttpContext.Request.Host.Host,

                Path = context.HttpContext.Request.Path.Value.TrimEnd('/'),
                Scheme = context.HttpContext.Request.Scheme

            };
            if (request.Host.Port.HasValue)
            {
                uri.Port = request.Host.Port.Value;
            }
            var response = context.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            response.Headers.Add("Location", uri.ToString());
            context.Result = RuleResult.EndResponse;
        }
    }
}