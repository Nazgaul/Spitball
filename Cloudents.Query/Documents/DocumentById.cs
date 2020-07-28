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
                    //.Fetch(f => f.User).ThenFetch(x => ((User) x).Tutor)
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => new DocumentDetailDto()
                    {
                        Title = s.Name,
                        //DateTime = s.TimeStamp.UpdateTime,
                        Id = s.Id,
                        UserId = ((User)s.User).Tutor!.Id,
                        //Duration = s.Duration,
                        UserName = ((User) s.User).Name,
                        DocumentType = s.DocumentType,
                        Pages = s.PageCount ?? 0,
                        Price = s.Course.Price,
                        SubscriptionPrice = s.Course.SubscriptionPrice
                        //PriceType = s.DocumentPrice.Type
                    }).ToFutureValue();


                if (!query.UserId.HasValue)
                {
                    return await futureValue.GetValueAsync(token);
                }


                var purchaseFuture = _session.Query<CourseEnrollment>()
                    .Where(w => w.User.Id == query.UserId.Value
                                && w.Course.Id == _session.Query<Document>()
                                    .Single(w2 => w2.Id == query.Id && w2.Status.State == ItemState.Ok)
                                    .Course.Id
                    )
                    .ToFutureValue(f => f.Any());


                //var scribedQueryFuture = _session.Query<Follow>()
                //      .Where(w => w.Follower.Id == query.UserId)
                //      .Where(w => w.User.Id == _session.Query<Document>().Where(w => w.Id == query.Id).Select(s => s.User.Id).Single())
                //      //.Where(w => w.User.Id == query.Id)
                //      .Select(s => s.Subscriber).ToFutureValue();





                var result = await futureValue.GetValueAsync(token);


                if (result == null)
                {
                    return null;
                }
                //result.IsPurchased = true;

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
                //else
                //{
                //    if (result.UserId == query.UserId.Value)
                //    {
                //        result.IsPurchased = true;
                //    }
                //    else
                //    {
                //        var transactionResult = await purchaseFuture.GetValueAsync(token);
                //        result.IsPurchased = scribedQueryFuture.Value ?? transactionResult != null;
                //    }
                //}
                //return result;
            }
        }
    }
}