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

                var tutorCountFuture = _session.QueryOver<Core.Entities.Tutor>()
                    //.Where(w => w.Status.State == ItemState.Ok)
                    .ToRowCountQuery().FutureValue<int>();




                var documentCount = await documentCountFuture.GetValueAsync(token);
                var questionCount = await questionCountFuture.GetValueAsync(token);
                var tutorCount = await tutorCountFuture.GetValueAsync(token);
                return new List<SiteMapCountDto>
                {
                    new SiteMapCountDto(SeoType.Document, documentCount),
                    new SiteMapCountDto(SeoType.Question,questionCount),
                    new SiteMapCountDto(SeoType.Tutor, tutorCount)
                };
            }
        }
    }
}