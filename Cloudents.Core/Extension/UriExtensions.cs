using System;

namespace Cloudents.Core.Extension
{
    public static class UriExtensions
    {
        //public static Uri ChangeToHttps(this Uri uri)
        //{
        //    if (uri.Scheme == Uri.UriSchemeHttps)
        //    {
        //        return uri;
        //    }
        //    var uriBuilder = new UriBuilder(uri)
        //    {
        //        Scheme = Uri.UriSchemeHttps,
        //        Port = -1
        //    };
        //    return uriBuilder.Uri;
        //}

        public static Uri ChangeHost(this Uri uri, string newHost)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                Host = newHost
            };
            return uriBuilder.Uri;
        }

        //public static string GetUriDomain(this Uri value)
        //{
        //    var host = value.Host;
        //    var lastDot = host.LastIndexOf('.');

        //    var secondToLastDot = host.Substring(0, lastDot).LastIndexOf('.');

        //    if (secondToLastDot > -1)
        //        return host.Substring(secondToLastDot + 1);
        //    return host;
        //}
    }
}