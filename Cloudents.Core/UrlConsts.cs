using System;
using System.Collections.Specialized;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core
{
    [UsedImplicitly]
    public class UrlConst : IUrlBuilder
    {
        private readonly Uri _webSiteEndPoint;
        private readonly Uri _functionEndPoint;

        public UrlConst(IConfigurationKeys configuration)
        {
            var siteEndPoints = configuration.SiteEndPoint;
            if (siteEndPoints.SpitballSite != null)
            {
                _webSiteEndPoint = new Uri(siteEndPoints.SpitballSite.TrimEnd('/'));
            }
            if (siteEndPoints.FunctionSite != null)
            {
                _functionEndPoint = new Uri(siteEndPoints.FunctionSite.TrimEnd('/'));
            }
           
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

        public string BuildCourseEndPoint(string courseName)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = "ask" };
            builder.AddQuery(new { Course = courseName });
            return builder.ToString();
        }

        public string BuildQuestionEndPoint
            (long id, object parameters = null)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"question/{id}" };
            builder.AddQuery(parameters);
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
            nvc.Add("channel", CommunicationChannel.Phone.ToString("G"));
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

        public string BuildDocumentEndPoint(long id, object parameters = null)
        {
            var base62 = new Base62(id);
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"document/{base62.ToString()}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }


        public const string ImageFunctionDocumentRoute = "image/document/{id}";
        public string BuildDocumentThumbnailEndpoint(long id)
        {
            var path = ImageFunctionDocumentRoute.InjectSingleValue("id", id);
            var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
            return builder.ToString();
        }
       
    }
}
