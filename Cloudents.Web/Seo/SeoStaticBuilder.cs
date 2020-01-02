using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Seo
{
    public class SeoStaticBuilder : IBuildSeo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoStaticBuilder(IHttpContextAccessor httpContextAccessor)
        {
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