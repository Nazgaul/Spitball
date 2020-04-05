using System;
using System.Collections.Generic;
using Cloudents.Core.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cloudents.Web.Seo
{
    public class SeoStaticBuilder : IBuildSeo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public SeoStaticBuilder(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public IEnumerable<SitemapNode> GetUrls(bool isFrymo, int index)
        {
            yield return new SitemapNode(GetBaseUrl())
            {
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1,
                TimeStamp = DateTime.UtcNow
            };

            yield return new SitemapNode(_linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Static, new
            {
                Id = "learn",
            }));
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