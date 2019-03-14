using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Views
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Using in razor page")]
    public static class Constants
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Using in razor page")]
        public static string GetCdnEndpoint(IConfiguration env)
        {
            return env["cdnEndpoint"] ?? string.Empty;
        }
    }
}