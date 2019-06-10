using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    [UsedImplicitly]
    public class UrlConst : IUrlBuilder
    {
        public const string GeneratePaymentLink = "generatePaymentLink";
        private readonly Uri _webSiteEndPoint;

        public UrlConst(IConfigurationKeys configuration)
        {
            var siteEndpoint = configuration.SiteEndPoint;
            if (siteEndpoint.EndsWith("/"))
            {
                siteEndpoint = siteEndpoint.Remove(siteEndpoint.Length - 1);
            }
            _webSiteEndPoint = new Uri(siteEndpoint);
        }

        public string BuildWalletEndPoint(string token)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = "wallet" };
            builder.AddQuery(new { token });
            return builder.ToString();
        }

        public string BuildShareEndPoint(string token)
        {
            var builder = new UriBuilder(_webSiteEndPoint);
            builder.AddQuery(new { token, open = "referral" });
            return builder.ToString();
        }

        public string BuildQuestionEndPoint
            (long id, object parameters = null)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"question/{id}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }

        public string BuildPayMeBuyerEndPoint(string token)
        {
           
            var builder = new UriBuilder(_webSiteEndPoint) { Path = GeneratePaymentLink };
            builder.AddQuery(token);
            return builder.ToString();
        }

        public Uri BuildChatEndpoint(string token, object parameters = null)
        {
            var nvc = new NameValueCollection();
            
            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetProperties())
                    nvc.Add(property.Name, property.GetValue(parameters).ToString());
            }
            nvc.Add("token",token);
            nvc.Add("chat", "expand");


            var builder = new UriBuilder(_webSiteEndPoint);
            builder.AddQuery(nvc);
            return builder.Uri;
        }

        public Uri BuildShortUrlEndpoint(string identifier)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"go/{identifier}" };
            return builder.Uri;
        }

        //public string BuildDocumentEndPoint(long id, object parameters = null)
        //{
        //    var base62 = new Base62(id);
        //    var builder = new UriBuilder(_webSiteEndPoint) { Path = $"document/{base62.ToString()}" };
        //    builder.AddQuery(parameters);
        //    return builder.ToString();
        //}

        public string BuildRedirectUrl(string url, string host, int? location)
        {
            if (host.Contains("spitball", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }

            if (url.Contains("spitball", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            var nvc = new NameValueCollection
            {
                ["url"] = url,
                ["host"] = host,

            };
            if (location.HasValue)
            {
                nvc["location"] = location.ToString();
            }

            var uri = new UriBuilder(_webSiteEndPoint)
            {
                Path = "url"
            };
            uri.AddQuery(nvc);
            return uri.ToString();
        }
    }
}
