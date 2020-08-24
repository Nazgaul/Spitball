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


                Core.Entities.Tutor tutorAlias = null!;
                Course courseAlias = null!;
                User userAlias = null!;



                var tutorCoursesFutureQuery = _session.QueryOver(() => tutorAlias)
                    .JoinAlias(x => x.Courses, () => courseAlias)
                    .JoinAlias(x => x.User, () => userAlias)
                    .SelectList(t => t.SelectCountDistinct(() => courseAlias.Name));
                if (query.IsFrymo)
                {
                    tutorCoursesFutureQuery.Where(() => userAlias.SbCountry == Country.India);
                }
                else
                {
                    tutorCoursesFutureQuery.Where(() => userAlias.SbCountry != Country.India);

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