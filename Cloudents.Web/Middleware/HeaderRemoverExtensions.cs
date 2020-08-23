using System.Collections.Immutable;
using Microsoft.AspNetCore.Builder;

namespace Cloudents.Web.Middleware
{
    public static class HeaderRemoverExtensions
    {
        public static IApplicationBuilder UseHeaderRemover(this IApplicationBuilder builder, params string[] headersToRemove)
        {
            return builder.UseMiddleware<HeaderRemoverMiddleware>(headersToRemove.ToImmutableList());
        }
    }
}