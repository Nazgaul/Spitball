using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Web.Seo
{
    public class SeoCourseTutorBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoCourseTutorBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }


        public IEnumerable<SitemapNode> GetUrls(int index)
        {

            var query = from tutor in _session.Query<Tutor>().Fetch(f => f.User)
                        join userCourse in _session.Query<UserCourse>() on tutor.Id equals userCourse.User.Id
                        where tutor.State == ItemState.Ok && userCourse.CanTeach && tutor.User.Country != Country.India.Name
                        select userCourse.Course.Id;
            foreach (var course in query.Distinct())
            {
                var url = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.TutorList, new
                {
                    term = course
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
