using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;

namespace Cloudents.Web.Seo
{
    public class SeoDocumentBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;

        private readonly TelemetryClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoDocumentBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor, TelemetryClient client)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _client = client;
        }

        public IEnumerable<SitemapNode> GetUrls(bool isFrymo, int index)
        {
            var t = _session.Query<Document>()
                //.Fetch(f => f.University)
                .Where(w => w.Status.State == ItemState.Ok);

            if (isFrymo)
            {
                t = t.Where(w => w.User.SbCountry == Country.India);
            }
            else
            {
                t = t.Where(w => w.User.SbCountry != Country.India);
            }

            var docs = t.Take(SiteMapController.PageSize).Skip(SiteMapController.PageSize * index)
                 .Select(s => new
                 {
                     s.Id,
                     s.Name,
                     CourseName = s.Course.Id,
                    // UniversityName = s.University.Name,
                    s.TimeStamp.UpdateTime

                 });

            foreach (var item in docs)
            {
                var url = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Document, new
                {
                    courseName = FriendlyUrlHelper.GetFriendlyTitle(item.CourseName),
                    item.Id,
                    name = FriendlyUrlHelper.GetFriendlyTitle(item.Name)
                });
                if (string.IsNullOrEmpty(url))
                {
                    _client.TrackTrace("Fail to Generate Doc Url", SeverityLevel.Critical, new Dictionary<string, string>()
                    {
                        ["Id"] = item.Id.ToString()
                    });
                    continue;
                }

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