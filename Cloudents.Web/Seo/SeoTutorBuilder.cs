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

    public class SeoTutorBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoTutorBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<SitemapNode> GetUrls(int index)
        {
            var t = _session.Query<Tutor>()
                .Fetch(f => f.User)
                .Where(w => (!w.User.LockoutEnd.HasValue || DateTime.UtcNow >= w.User.LockoutEnd.Value))
                .Where(w => w.State == ItemState.Ok && w.User.Country != Country.India.Name)
                .Take(SiteMapController.PageSize)
                .Skip(SiteMapController.PageSize * index)
                .Select(s => new { s.Id, s.User.Name });

            foreach (var item in t)
            {



                var url =  _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Tutor, new
                {
                    item.Id,
                    item.Name
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