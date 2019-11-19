using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Web.Views
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Using in razor page")]
    public static class Constants
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Using in razor page")]
        public static string GetCdnEndpoint(IConfiguration env)
        {
            return env["cdnEndpoint"] ?? string.Empty;
        }
    }
}