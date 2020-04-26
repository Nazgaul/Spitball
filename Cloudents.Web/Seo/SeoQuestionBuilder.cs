using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Web.Seo
{
    public class SeoQuestionBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoQuestionBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<SitemapNode> GetUrls(bool isFrymo, int index)
        {
            var t = _session.Query<Question>()
                .Fetch(f => f.User)
                .Where(w => w.Status.State == ItemState.Ok);

            if (isFrymo)
            {
                t = t.Where(w => w.User.Country == Country.India.Name);
            }
            else
            {
                t = t.Where(w => w.User.Country != Country.India.Name);
            }

            var question = t.Take(SiteMapController.PageSize).Skip(SiteMapController.PageSize * index)
              .Select(s => s.Id);

            foreach (var item in question)
            {
                var url = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Question, new
                {
                    Id = item,
                });
                yield return new SitemapNode(url)
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1,
                    TimeStamp = DateTime.UtcNow
                };

            }
        }

        
    }
}