﻿using System;
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


        public IEnumerable<SitemapNode> GetUrls(bool isFrymo, int index)
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

            var t = _session.Query<Tutor>().Fetch(f => f.User)
                .FetchMany(f => f.Courses)
                //.Join(_session.Query<UserCourse>(), x => x.Id, z => z.User.Id, (tutor, course) => new
                //{
                //    tutor,
                //    course
                //})
                .Where(w => w.State == ItemState.Ok);
               //  .Where(w => w.course.IsTeach);

            if (isFrymo)
            {
                t = t.Where(w => w.User.Country == Country.India.Name);
            }
            else
            {
                t = t.Where(w => w.User.Country != Country.India.Name);
            }

            var query = t.GroupBy(x => x.Courses).Select(s => new { Course = s.Key, Count = s.Count() });
          
            foreach (var group in query)
            {
                for (var i = 0; i <= group.Count / pageSize; i++)
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
