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

                //Document documentAlias = null!;
                //ReadTutor tutorAlias = null!;
                //BaseUser userAlias = null!;
                //DocumentDetailDto dtoAlias = null!;

                //var similarDocumentQueryOver = QueryOver.Of<Document>()
                //    .Where(w => w.Md5 == documentAlias.Md5 && w.Status.State == ItemState.Ok)
                //    .And(x => x.Md5 != null).OrderBy(o => o.Id).Asc.Select(s => s.Id).Take(1);

                var futureValue = _session.Query<Document>()
                    .Fetch(f => f.User).ThenFetch(x => ((User) x).Tutor)
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => new DocumentDetailDto()
                    {
                        Title = s.Name,
                        DateTime = s.TimeStamp.UpdateTime,
                        Id = s.Id,
                        UserId = ((User) s.User).Tutor!.Id,
                        Duration = s.Duration,
                        UserName = ((User) s.User).Name,
                        DocumentType = s.DocumentType,
                        Pages = s.PageCount ?? 0,
                        Price = s.DocumentPrice.Price,
                        PriceType = s.DocumentPrice.Type
                    }).ToFutureValue();


                //var futureValue = _session.QueryOver(() => documentAlias)
                //    .JoinAlias(x => x.User, () => userAlias)
                //    .JoinEntityAlias(() => tutorAlias, () => documentAlias.User.Id == tutorAlias.Id, JoinType.LeftOuterJoin)
                //    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                //    .SelectList(l =>
                //        l.Select(() => documentAlias.PageCount).WithAlias(() => dtoAlias.Pages)
                //            .Select(Projections.Property(() => documentAlias.Id).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Id)}"))
                //            .Select(Projections.Property(() => documentAlias.Name).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Title)}"))
                //            .Select(Projections.Property(() => documentAlias.TimeStamp.UpdateTime).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.DateTime)}"))
                //            .Select(Projections.Property(() => documentAlias.DocumentPrice.Price).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.Price)}"))
                //            .Select(Projections.Property(() => documentAlias.DocumentPrice.Type).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.PriceType)}"))
                //            .Select(Projections.Property(() => documentAlias.DocumentType).As($"{nameof(DocumentDetailDto.Document)}.{nameof(DocumentFeedDto.DocumentType)}"))


                //    )
                //    .TransformUsing(new DeepTransformer<DocumentDetailDto>())
                //    .UnderlyingCriteria.SetComment(nameof(DocumentById))
                //    .FutureValue<DocumentDetailDto>();


                if (!query.UserId.HasValue)
                {
                    return await futureValue.GetValueAsync(token);
                }
                var purchaseFuture = _session.QueryOver<DocumentTransaction>()
                       .Where(w => w.User.Id == query.UserId.Value && w.Document.Id == query.Id && w.Type == TransactionType.Spent)
                       .UnderlyingCriteria.SetComment(nameof(DocumentById))
                       .FutureValue<DocumentTransaction>();


                var scribedQueryFuture = _session.Query<Follow>()
                      .Where(w => w.Follower.Id == query.UserId)
                      .Where(w => w.User.Id == _session.Query<Document>().Where(w => w.Id == query.Id).Select(s => s.User.Id).Single())
                      //.Where(w => w.User.Id == query.Id)
                      .Select(s => s.Subscriber).ToFutureValue();





                var result = await futureValue.GetValueAsync(token);


                if (result == null)
                {
                    return null;
                }
                result.IsPurchased = true;

                if (result.Price.GetValueOrDefault() <= 0) return result;
                if (purchaseFuture == null)
                {
                    result.IsPurchased = false;

                }
                else
                {
                    if (result.UserId == query.UserId.Value)
                    {
                        result.IsPurchased = true;
                    }
                    else
                    {
                        var transactionResult = await purchaseFuture.GetValueAsync(token);
                        result.IsPurchased = scribedQueryFuture.Value ?? transactionResult != null;
                    }
                }
                return result;
            }
        }
    }
}