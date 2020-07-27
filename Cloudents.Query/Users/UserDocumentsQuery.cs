using System.Collections.Generic;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.EventHandler;
using Cloudents.Core.Interfaces;

namespace Cloudents.Query.Users
{
    public class UserDocumentsQuery : IQuery<IDictionary<string,List<DocumentFeedDto>>>
    {
        public UserDocumentsQuery(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private long Id { get; }


        private long UserId { get; }

        internal sealed class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, IDictionary<string,List<DocumentFeedDto>>>
        {
            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;

            public UserDocumentsQueryHandler(IStatelessSession session, IUrlBuilder urlBuilder)
            {
                _session = session;
                _urlBuilder = urlBuilder;
            }
            [Cache(TimeConst.Minute * 30, CacheRegions.ProfilePageDocument, false)]
            public async Task<IDictionary<string,List<DocumentFeedDto>>> GetAsync(UserDocumentsQuery query, CancellationToken token)
            {
                var r = _session.Query<Document>()

                    .WithOptions(w => w.SetComment(nameof(UserDocumentsQuery)))
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok);

                r = r.OrderByDescending(o => o.TimeStamp.UpdateTime);
                var result = r.Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    Title = s.Name,
                    Course = s.Course.Name,
                   // Snippet = s.Description ?? s.MetaContent,
                   // Price = s.DocumentPrice.Price,
                   // PriceType = s.DocumentPrice.Type ?? PriceType.Free,
                    DocumentType = s.DocumentType,
                   // Duration = s.Duration,
                    Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s.Id, null)
                }).ToFuture();


                IFutureValue<bool?>? scribedQueryFuture = null;
                if (query.UserId > 0)
                {
                    scribedQueryFuture = _session.Query<Follow>()
                       .Where(w => w.Follower.Id == query.UserId)
                       .Where(w => w.User.Id == query.Id)
                       .Select(s => s.Subscriber).ToFutureValue();

                }


                var futureResult = await result.GetEnumerableAsync(token);
                var isSubscribed = scribedQueryFuture?.Value ?? false;

                if (isSubscribed)
                {
                    futureResult = futureResult.Select(s =>
                    {
                       // s.PriceType = PriceType.Free;
                       // s.Price = null;
                        return s;
                    });
                }
                
                return futureResult.GroupBy(g => g.Course).ToDictionary(x=>x.Key,z=>z.ToList());

            }
        }
    }
}
