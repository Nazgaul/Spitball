using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
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
            var urlBase = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.TutorList, new
            {
            });
            yield return new SitemapNode(urlBase)
            {
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1,
                TimeStamp = DateTime.UtcNow
            };

            const int pageSize = 50;
            var query = from tutor in _session.Query<Tutor>().Fetch(f => f.User)
                        join userCourse in _session.Query<UserCourse>() on tutor.Id equals userCourse.User.Id
                        where tutor.State == ItemState.Ok
                              && userCourse.CanTeach && tutor.User.Country != Country.India.Name
                        group userCourse by userCourse.Course.Id
                into newGroup
                        select new { Course = newGroup.Key, Count = newGroup.Count() };
            foreach (var group in query)
            {
                for (int i = 0; i < group.Count / pageSize; i++)
                {
                    var url = _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.TutorList, new
                    {
                        term = group.Course,
                        page = i,
                        size = pageSize
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
}
