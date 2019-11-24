using System;
using System.Collections.Generic;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cloudents.Web.Seo
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

        public IEnumerable<SitemapNode> GetUrls(int index)
        {
            yield return new SitemapNode(GetBaseUrl())
            {
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1,
                TimeStamp = DateTime.UtcNow
            }; 
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