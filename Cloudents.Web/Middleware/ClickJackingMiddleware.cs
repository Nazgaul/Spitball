using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Middleware
{
    public class ClickJackingMiddleware
    {
        private readonly RequestDelegate _next;

        public ClickJackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("X-Frame-Options", "sameorigin");
            return _next.Invoke(context);
        }
    }
}