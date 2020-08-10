using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.EventHandler;
using NHibernate.Linq;

namespace Cloudents.Query.Documents
{
    public class DocumentById : IQuery<DocumentDetailDto?>
    {
        public DocumentById(long id, long? userId)
        {
            Id = id;
            UserId = userId;
        }



        public long Id { get; }
        private long? UserId { get; }


        internal sealed class DocumentByIdQueryHandler : IQueryHandler<DocumentById, DocumentDetailDto?>
        {
            private readonly IStatelessSession _session;

            public DocumentByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }
            [Cache(TimeConst.Minute * 2, CacheRegions.DocumentById, false)]
            public async Task<DocumentDetailDto?> GetAsync(DocumentById query, CancellationToken token)
            {
                var futureValue = _session.Query<Document>()
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => new DocumentDetailDto()
                    {
                        Title = s.Name,
                        Id = s.Id,
                        UserId = s.User.Id,
                        UserName = s.User.Name,
                        DocumentType = s.DocumentType,
                        Pages = s.PageCount ?? 0,
                        Price = s.Course.Price,
                        SubscriptionPrice = s.Course.SubscriptionPrice,
                        CourseId = s.Course.Id
                    }).ToFutureValue();


                if (!query.UserId.HasValue)
                {
                    return await futureValue.GetValueAsync(token);
                }


                var purchaseFuture = _session.Query<CourseEnrollment>()
                    .Where(w => w.User.Id == query.UserId.Value
                                && w.Course.Id == _session.Query<Document>()
                                    .Where(w2 => w2.Id == query.Id 
                                            && w2.Status.State == ItemState.Ok).Select(s=>s.Course.Id).FirstOrDefault()
                    )
                    .ToFutureValue(f => f.Any());

                var result = await futureValue.GetValueAsync(token);


                if (result == null)
                {
                    return null;
                }

                if (result.UserId == query.UserId)
                {
                    result.IsPurchased = true;
                    return result;
                }

                if (result.Price.Cents == 0)
                {
                    result.IsPurchased = true;
                    return result;
                }
                if (purchaseFuture.Value)
                {
                    result.IsPurchased = true;
                    return result;

                }

                return result;

            }
        }
    }
}