using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class UserSoldDocsQuery : IQueryAdmin<IEnumerable<UserSoldItemsDto>>
    {
        public UserSoldDocsQuery(long userId, int page, string country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }

        public long UserId { get; }
        public int Page { get; }
        public string Country { get; }

        internal sealed class UserSoldDocsQueryHandler : IQueryHandler<UserSoldDocsQuery, IEnumerable<UserSoldItemsDto>>
        {
            private readonly IStatelessSession _session;


            public UserSoldDocsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<UserSoldItemsDto>> GetAsync(UserSoldDocsQuery query, CancellationToken token)
            {
                const int pageSize = 20;
                User userAlias = null;
                DocumentTransaction transactionAlias = null;
                Document documentAlias = null;

                UserSoldItemsDto userSoldDto = null;

                var cte = QueryOver.Of(() => documentAlias)
                    .Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Id);

                var userCountry = QueryOver.Of(() => userAlias)
                    .Where(w => w.Id == query.UserId)
                    .Select(s => s.Country);

                var t = _session.QueryOver(() => transactionAlias)
                    .Inner.JoinAlias(r => r.Document, () => documentAlias, r => r.Id == transactionAlias.Document.Id)
                    .Inner.JoinAlias(r => r.User, () => userAlias, r => r.Id == transactionAlias.User.Id)
                    .WithSubquery.WhereProperty(w => w.Document.Id).In(cte);

                if (!string.IsNullOrEmpty(query.Country))
                {
                    t.WithSubquery.WhereValue(query.Country).Eq(userCountry);
                }

                return await t.Where(w => w.User.Id != query.UserId)
                    .SelectList(s =>
                        s.Select(x => x.Created).WithAlias(() => userSoldDto.TransactionTime)
                        .Select(x => x.Price).WithAlias(() => userSoldDto.TransactionPrice)
                        .Select(x => x.Document.Id).WithAlias(() => userSoldDto.ItemId)
                        .Select(x => documentAlias.Name).WithAlias(() => userSoldDto.ItemName)
                        .Select(x => documentAlias.TimeStamp.CreationTime).WithAlias(() => userSoldDto.ItemCreated)
                        .Select(x => documentAlias.Course.Id).WithAlias(() => userSoldDto.ItemCourse)
                        .Select(x => documentAlias.Status.State).WithAlias(() => userSoldDto.ItemState)
                        .Select(x => documentAlias.DocumentType).WithAlias(() => userSoldDto.ItemType)
                        .Select(x => userAlias.Name).WithAlias(() => userSoldDto.PurchasedUserName)
                        .Select(x => userAlias.Email).WithAlias(() => userSoldDto.PurchasedUserEmail)
                        .Select(x => userAlias.Transactions.Balance).WithAlias(() => userSoldDto.PurchasedUserBalance)
                        ).TransformUsing(Transformers.AliasToBean<UserSoldItemsDto>())
                        .Skip(query.Page * pageSize).Take(pageSize).ListAsync<UserSoldItemsDto>();
            }
        }
    }
}

