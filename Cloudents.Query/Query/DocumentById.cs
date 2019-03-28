using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class DocumentById : IQuery<DocumentDetailDto>
    {
        public DocumentById(long id, long? userId)
        {
            Id = id;
            UserId = userId;
        }



        public long Id { get; }
        public long? UserId { get; }


        internal sealed class DocumentByIdQueryHandler : IQueryHandler<DocumentById, DocumentDetailDto>
        {
            private readonly IStatelessSession _session;

            public DocumentByIdQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<DocumentDetailDto> GetAsync(DocumentById query, CancellationToken token)
            {
                var futureValue = _session.Query<Document>()

                   .Fetch(f => f.University)
                   .Fetch(f => f.User)
                   .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                   .Select(s => new DocumentDetailDto
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Date = s.TimeStamp.UpdateTime,
                       University = s.University.Name,
                       Type = s.Type,
                       Pages = s.PageCount.GetValueOrDefault(),
                       Professor = s.Professor,
                       Views = s.Views,
                       Downloads = s.Downloads,
                       Price = s.Price,
                       PageCount = s.PageCount.GetValueOrDefault(),
                       User = new UserDto
                       {
                           Id = s.User.Id,
                           Name = s.User.Name,
                           Image = s.User.Image,
                           Score = s.User.Score
                       },
                       Course = s.Course.Id
                   }).ToFutureValue();//.SingleOrDefaultAsync(token);

                if (!query.UserId.HasValue)
                {
                    return await futureValue.GetValueAsync(token);
                }
                var purchaseFuture = _session.Query<DocumentTransaction>()
                       .Where(w => w.User.Id == query.UserId.Value && w.Document.Id == query.Id && w.Type == TransactionType.Spent)
                       .ToFutureValue();

                var result = await futureValue.GetValueAsync(token);
                result.IsPurchased = true;
                if (result.Price.GetValueOrDefault() <= 0) return result;
                if (purchaseFuture == null)
                {
                    result.IsPurchased = false;

                }
                else
                {
                    if (result.User.Id == query.UserId.Value)
                    {

                        result.IsPurchased = true;
                    }
                    else
                    {
                        var transactionResult = await purchaseFuture.GetValueAsync(token);
                        result.IsPurchased = transactionResult != null;
                    }
                }
                return result;
            }
        }
    }
}