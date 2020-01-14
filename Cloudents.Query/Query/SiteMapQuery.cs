using System;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Stuff;
using NHibernate.Criterion;

namespace Cloudents.Query.Query
{
    public class SiteMapQuery : IQuery<IList<SiteMapCountDto>>
    {
        public SiteMapQuery(bool isFrymo)
        {
            IsFrymo = isFrymo;
        }

        private bool IsFrymo { get; }

        internal sealed class SeoItemCountQueryHandler : IQueryHandler<SiteMapQuery, IList<SiteMapCountDto>>
        {
            private readonly IStatelessSession _session;

            public SeoItemCountQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<SiteMapCountDto>> GetAsync(SiteMapQuery query, CancellationToken token)
            {

                
                University universityAlias = null;
                BaseUser userAlias = null;

                //var methodInfo = typeof(Restrictions).GetMethod("EqProperty", BindingFlags.Static | BindingFlags.Public);
                var documentCountFutureQuery = _session.QueryOver<Document>()
                    .Left.JoinAlias(x => x.University, () => universityAlias)
                    .JoinAlias(x => x.User, () => userAlias)
                    .Where(w => w.Status.State == ItemState.Ok);
                //.Where(() =>  universityAlias.Country.IfNull(userAlias.Country) == "IN")
                //.Where(FilterQuery(universityAlias,userAlias, query.IsFrymo));
                if (query.IsFrymo)
                {
                    documentCountFutureQuery.Where(() => universityAlias.Country.IfNull(userAlias.Country) == "IN");
                }
                else
                {
                    documentCountFutureQuery.Where(() => universityAlias.Country.IfNull(userAlias.Country) != "IN");

                }
                var documentCountFuture = documentCountFutureQuery.ToRowCountQuery().FutureValue<int>();


                var questionCountFutureQuery = _session.QueryOver<Question>()
                    .Left.JoinAlias(x => x.University, () => universityAlias)
                    .JoinAlias(x => x.User, () => userAlias)
                    .Where(w => w.Status.State == ItemState.Ok);

                if (query.IsFrymo)
                {
                    questionCountFutureQuery.Where(() => universityAlias.Country.IfNull(userAlias.Country) == "IN");
                }
                else
                {
                    questionCountFutureQuery.Where(() => universityAlias.Country.IfNull(userAlias.Country) != "IN");

                }

                //.Where(FilterQuery(universityAlias, userAlias,query.IsFrymo))

                //.Where(w => w.Status.State == ItemState.Ok)
                //.And(
                //    Restrictions.EqProperty(
                //        Projections.SqlFunction(
                //            "COALESCE",
                //            NHibernateUtil.String,
                //            Projections.Property(() => universityAlias.Country),
                //            Projections.Property(() => userAlias.Country)),
                //        Projections.Constant("IN")))
                var questionCountFuture = questionCountFutureQuery.ToRowCountQuery().FutureValue<int>();

                var tutorCountFutureQuery = _session.QueryOver<ReadTutor>();

                if (query.IsFrymo)
                {
                    tutorCountFutureQuery.Where(w=>w.Country == "IN");
                }
                else
                {
                    tutorCountFutureQuery.Where(w => w.Country != "IN");

                }
                //.Where(w => w.Country == "IN")
                var tutorCountFuture = tutorCountFutureQuery.ToRowCountQuery().FutureValue<int>();

                UserCourse userCourseAlias = null;
                Core.Entities.ReadTutor tutorAlias = null;





                var tutorCoursesFutureQuery = _session.QueryOver(() => tutorAlias)
                    .JoinEntityAlias(() => userCourseAlias, () => tutorAlias.Id == userCourseAlias.User.Id)
                    .Where(() => userCourseAlias.CanTeach)
                    .SelectList(t => t.SelectCountDistinct(() => userCourseAlias.Course.Id));
                if (query.IsFrymo)
                {
                    tutorCoursesFutureQuery.Where(w => w.Country == "IN");
                }
                else
                {
                    tutorCoursesFutureQuery.Where(w => w.Country != "IN");

                }
                    
                    
                    
                var tutorCoursesFuture  = tutorCoursesFutureQuery.FutureValue<int>();




                var documentCount = await documentCountFuture.GetValueAsync(token);
                var questionCount = await questionCountFuture.GetValueAsync(token);
                var tutorCount = await tutorCountFuture.GetValueAsync(token);
                var tutorSiteCount = await tutorCoursesFuture.GetValueAsync(token);
                return new List<SiteMapCountDto>
                {
                    new SiteMapCountDto(SeoType.Document, documentCount),
                    new SiteMapCountDto(SeoType.Question,questionCount),
                    new SiteMapCountDto(SeoType.Tutor, tutorCount),
                    new SiteMapCountDto(SeoType.TutorList, tutorSiteCount)
                };
            }




        }
    }
}