using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Cloudents.Query.General
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

            public SeoItemCountQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IList<SiteMapCountDto>> GetAsync(SiteMapQuery query, CancellationToken token)
            {



                //var documentCountFutureQuery = _session.QueryOver<Document>()
                //    .JoinAlias(x => x.User, () => userAlias)
                //    .Where(w => w.Status.State == ItemState.Ok);
                //if (query.IsFrymo)
                //{

                //    documentCountFutureQuery.Where(() => userAlias.SbCountry == Country.India);
                //}
                //else
                //{
                //    documentCountFutureQuery.Where(() => userAlias.SbCountry != Country.India);

                //}
                //var documentCountFuture = documentCountFutureQuery.ToRowCountQuery().UnderlyingCriteria.SetComment(nameof(SiteMapQuery)).FutureValue<int>();


                //var questionCountFutureQuery = _session.QueryOver<Question>()
                //    .JoinAlias(x => x.User, () => userAlias)
                //    .Where(w => w.Status.State == ItemState.Ok);

                //if (query.IsFrymo)
                //{
                //    questionCountFutureQuery.Where(() => userAlias.SbCountry == Country.India);
                //}
                //else
                //{
                //    //questionCountFutureQuery.Where(Restrictions.Eq(Projections.Property(()=> userAlias.SbCountry), Country.India));
                //    questionCountFutureQuery.Where(() => userAlias.SbCountry != Country.India);

                //}

                //var questionCountFuture = questionCountFutureQuery.ToRowCountQuery().FutureValue<int>();


                var tutorCountFutureQuery = _session.Query<Core.Entities.Tutor>();

                if (query.IsFrymo)
                {
                    tutorCountFutureQuery = tutorCountFutureQuery.Where(w => w.User.SbCountry == Country.India);
                }
                else
                {
                    tutorCountFutureQuery = tutorCountFutureQuery.Where(w => w.User.SbCountry != Country.India);

                }

                var tutorCountFuture = tutorCountFutureQuery.ToFutureValue(f => f.Count());


                UserCourse userCourseAlias = null!;
                Core.Entities.Tutor tutorAlias = null!;



                var tutorCoursesFutureQuery = _session.QueryOver(() => tutorAlias)
                    .JoinQueryOver(x => x.User)
                    .JoinEntityAlias(() => userCourseAlias, () => tutorAlias.Id == userCourseAlias.User.Id)
                    .Where(() => userCourseAlias.IsTeach)
                    .SelectList(t => t.SelectCountDistinct(() => userCourseAlias.Course.Id));
                if (query.IsFrymo)
                {
                    tutorCoursesFutureQuery.Where(w => w.SbCountry == Country.India);
                }
                else
                {
                    tutorCoursesFutureQuery.Where(w => w.SbCountry != Country.India);

                }



                var tutorCoursesFuture = tutorCoursesFutureQuery.FutureValue<int>();




                var tutorCount = await tutorCountFuture.GetValueAsync(token);
                var tutorSiteCount = await tutorCoursesFuture.GetValueAsync(token);
                return new List<SiteMapCountDto>
                {
                    new SiteMapCountDto(SeoType.Tutor, tutorCount),
                    new SiteMapCountDto(SeoType.TutorList, tutorSiteCount)
                };
            }




        }
    }
}