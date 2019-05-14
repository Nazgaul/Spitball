using System;
using System.Collections.Generic;
using Cloudents.Core.Enum;
using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cloudents.Web.Services
{
    public class SeoStaticBuilder : IBuildSeo
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoStaticBuilder(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<string> GetUrls(int index)
        {
            yield return GetBaseUrl();
            yield return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Static, new
            {
                id = "faq"
            });

        }

        private string GetBaseUrl()
        {
            var uriBuilder = new UriBuilder
            {
                Host = _httpContextAccessor.HttpContext.Request.Host.Host,
                Scheme = _httpContextAccessor.HttpContext.Request.Scheme
            };
            if (_httpContextAccessor.HttpContext.Request.Host.Port.HasValue)
            {
                uriBuilder.Port = _httpContextAccessor.HttpContext.Request.Host.Port.Value;
            }
            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}