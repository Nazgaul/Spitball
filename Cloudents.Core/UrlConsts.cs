﻿using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Specialized;
using Cloudents.Core.Entities;

namespace Cloudents.Core
{
    public class UrlConst : IUrlBuilder
    {
        private readonly Uri? _webSiteEndPoint;
        private readonly Uri? _functionEndPoint;
      //  private readonly Uri? _indiaWebSiteEndpoint;

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

            //if (siteEndPoints.IndiaSite != null)
            //{
            //    _indiaWebSiteEndpoint = new Uri(siteEndPoints.IndiaSite.TrimEnd('/'));
            //}


        }

        //public string BuildWalletEndPoint(string token)
        //{
        //    var builder = new UriBuilder(_webSiteEndPoint) { Path = "my-sales" };
        //    builder.AddQuery(new { token });
        //    return builder.ToString();
        //}

        //public string BuildShareEndPoint(string token)
        //{
        //    var builder = new UriBuilder(_webSiteEndPoint);
        //    builder.AddQuery(new { token, open = "referral" });
        //    return builder.ToString();
        //}

        public string BuildCourseEndPoint(string courseName)
        {
            var builder = new UriBuilder(_webSiteEndPoint);
            return builder.ToString();
        }



        public string BuildProfileEndPoint(long id)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"p/{id}" };
            return builder.ToString();
        }

        public Uri BuildChatEndpoint(string token,string identifier,  object? parameters = null)
        {
            var nvc = new NameValueCollection();

            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetProperties())
                    nvc.Add(property.Name, property.GetValue(parameters).ToString());
            }
            nvc.Add("token", token);
            nvc.Add("channel", CommunicationChannel.Email.ToString("G"));
            //nvc.Add("chat", "expand");


            var builder = new UriBuilder(_webSiteEndPoint) {Path = $"messages/{identifier}" };
            builder.AddQuery(nvc);
            return builder.Uri;
        }

        public Uri BuildShortUrlEndpoint(string identifier, Country? country)
        {
            var nvc = new NameValueCollection();
            var webSiteEndpoint = _webSiteEndPoint;
            //if (country == Country.India)
            //{
            //    webSiteEndpoint = _indiaWebSiteEndpoint;
            //}

            var builder = new UriBuilder(webSiteEndpoint) { Path = $"go/{identifier}" };
            builder.AddQuery(nvc);
            return builder.Uri;
        }



        public Uri BuildShortUrlEndpoint(string identifier, object? parameters = null)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"go/{identifier}" };
            builder.AddQuery(parameters);
            return builder.Uri;
        }

        public string BuildDocumentEndPoint(long id, object? parameters = null)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"d/{id}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }


        public const string ImageFunctionDocumentRoute = "image/document/{id}";
        public string BuildCourseThumbnailEndPoint(long id,int version, object? parameters = null)
        {
            var path = $"image/studyRoom/{id}";//
            var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
            builder.AddQuery(parameters);
            builder.AddQuery(("version", version.ToString()));
            return builder.ToString();
        }

        public string BuildDocumentThumbnailEndpoint(long id, object? parameters = null)
        {
            var path = ImageFunctionDocumentRoute.InjectSingleValue("id", id);
            var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }


        //public string BuildStudyRoomThumbnailEndPoint(Guid id, object? parameters = null)
        //{
        //    var path = $"image/studyRoom/{id}";//
        //    var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
        //    builder.AddQuery(parameters);
        //    return builder.ToString();
        //}



        public const string ImageFunctionUserRoute = "image/user/{id}/{file}";
        //public const string ImageFunctionStudyRoomRoute = "image/studyroom/{id}";
        public string BuildUserImageEndpoint(long id, string? imageName, string? userName, object? parameters = null)
        {

            var injectionObj = new
            {
                id,
                file = !string.IsNullOrEmpty(imageName) ? imageName : userName
            };
            var path = ImageFunctionUserRoute.Inject(injectionObj);
            var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }

        public string? BuildUserImageEndpoint(long id, string? imageName)
        {
            if (imageName is null)
            {
                return null;
            }

            if (Uri.TryCreate(imageName, UriKind.Absolute, out var uri))
            {
                return uri.AbsoluteUri;
            }

            return BuildUserImageEndpoint(id, imageName, null);
        }

        public Uri BuildTwilioWebHookEndPoint(Guid id)
        {
            var builder = new UriBuilder(_functionEndPoint) { Path = "api/roomCallback" };
            builder.AddQuery(new { id });
            return builder.Uri;
        }

        public string BuildStudyRoomEndPoint(Guid id, object? parameters = null)
        {
            var builder = new UriBuilder(_webSiteEndPoint) { Path = $"StudyRoom/{id}" };
            builder.AddQuery(parameters);
            builder.AddQuery(new {channel = CommunicationChannel.Email.ToString("G")});
            return builder.ToString();
        }


        private const string ImageFunctionUserProfileShareRoute = "share/profile/{id}";

        public string BuildUserImageProfileShareEndpoint(long id, object? parameters = null)
        {
            var injectionObj = new
            {
                id,
            };
            var path = ImageFunctionUserProfileShareRoute.Inject(injectionObj);
            var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }


        private const string ImageFunctionDocumentShareRoute = "share/document/{id}";

        public string BuildDocumentImageShareEndpoint(long id, object? parameters = null)
        {
            var injectionObj = new
            {
                id,
            };
            var path = ImageFunctionDocumentShareRoute.Inject(injectionObj);
            var builder = new UriBuilder(_functionEndPoint) { Path = $"api/{path}" };
            builder.AddQuery(parameters);
            return builder.ToString();
        }
    }
}
