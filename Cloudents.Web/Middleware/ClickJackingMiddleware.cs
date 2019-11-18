using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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