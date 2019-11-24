using System.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class SiteMapQuery : IQuery<IList<SiteMapCountDto>>
    {


        internal sealed class SeoItemCountQueryHandler : IQueryHandler<SiteMapQuery, IList<SiteMapCountDto>>
        {
            private readonly IStatelessSession _session;

            public SeoItemCountQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<SiteMapCountDto>> GetAsync(SiteMapQuery query, CancellationToken token)
            {
                
                var documentCountFuture = _session.QueryOver<Document>()
                    .Where(w => w.Status.State == ItemState.Ok)
                    .ToRowCountQuery().FutureValue<int>();


                var questionCountFuture = _session.QueryOver<Question>()
                    .Where(w => w.Status.State == ItemState.Ok)
                    .ToRowCountQuery().FutureValue<int>();

                var tutorCountFuture = _session.QueryOver<ReadTutor>()
                    .ToRowCountQuery().FutureValue<int>();

                UserCourse userCourseAlias = null;
                Core.Entities.Tutor tutorAlias = null;


              


                var tutorCoursesFuture = _session.QueryOver(() => tutorAlias)
                    .JoinEntityAlias(() => userCourseAlias, () => tutorAlias.Id == userCourseAlias.User.Id)
                    .Where(() => tutorAlias.State == ItemState.Ok)
                    .And(() => userCourseAlias.CanTeach)
                    .SelectList(t=>t.SelectCountDistinct(() => userCourseAlias.Course.Id))
                    .FutureValue<int>();
                    
                   


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