﻿using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Middleware
{
    public class HeaderRemoverMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ImmutableList<string> _headersToRemove;

        public HeaderRemoverMiddleware(RequestDelegate next, ImmutableList<string> headersToRemove)
        {
            _next = next;
            _headersToRemove = headersToRemove;
        }

        public Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.OnStarting(() =>
            {
                _headersToRemove.ForEach(header =>
                {
                    if (httpContext.Response.Headers.ContainsKey(header))
                    {
                        httpContext.Response.Headers.Remove(header);
                    }
                });

                return Task.CompletedTask;
            });

            return _next.Invoke(httpContext);
        }
    }
    public static class HeaderRemoverExtensions
    {
        public static IApplicationBuilder UseHeaderRemover(this IApplicationBuilder builder, params string[] headersToRemove)
        {
            return builder.UseMiddleware<HeaderRemoverMiddleware>(headersToRemove.ToImmutableList());
        }
    }
}
