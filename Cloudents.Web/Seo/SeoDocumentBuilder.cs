using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Web.Seo
{
    public class SeoDocumentBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoDocumentBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<SitemapNode> GetUrls(int index)
        {
            var t = _session.Query<Document>()
                .Fetch(f => f.University)
                .Where(w => w.Status.State == ItemState.Ok && w.University.Country != Country.India.Name)
                .Take(SiteMapController.PageSize).Skip(SiteMapController.PageSize * index)
                .Select(s => new 
                {
                    Id = s.Id,
                    Name = s.Name,
                    CourseName = s.Course.Id,
                    UniversityName = s.University.Name,
                    s.TimeStamp.UpdateTime
                    
                });

            foreach (var item in t)
            {
                var url = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Document, new
                {
                    courseName = FriendlyUrlHelper.GetFriendlyTitle(item.CourseName),
                    item.Id,
                    name = FriendlyUrlHelper.GetFriendlyTitle(item.Name)
                });

                yield return new SitemapNode(url)
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1,
                    TimeStamp = item.UpdateTime
                };

            }
        }
    }
}