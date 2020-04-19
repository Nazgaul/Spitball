using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    public class UserSoldDocsQuery : IQueryAdmin2<IEnumerable<UserSoldItemsDto>>
    {
        public UserSoldDocsQuery(long userId, int page, Country? country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }

        private long UserId { get; }
        private int Page { get; }
        public Country? Country { get; }

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

                var v = _session.Query<DocumentTransaction>()
                    .Fetch(f => f.User)
                    .Fetch(f => f.Document)
                    .Where(w => w.Document.User.Id == query.UserId && w.Type == TransactionType.Spent);

                if (query.Country != null)
                {
                    v = v.Where(w => w.User.SbCountry == query.Country);
                }

                return await v.Skip(query.Page * pageSize).Take(pageSize).Select(s => new UserSoldItemsDto
                {
                    //s.Select(x => x.Created).WithAlias(() => userSoldDto.TransactionTime)
                    //    .Select(x => x.Price).WithAlias(() => userSoldDto.TransactionPrice)
                    //    .Select(x => documentAlias.Name).WithAlias(() => userSoldDto.ItemName)
                    //    .Select(x => userAlias.Name).WithAlias(() => userSoldDto.PurchasedUserName)
                    //    .Select(x => userAlias.Email).WithAlias(() => userSoldDto.PurchasedUserEmail)
                    //    .Select(x => userAlias.Transactions.Balance).WithAlias(() => userSoldDto.PurchasedUserBalance)

                    ItemCourse = s.Document.Course2.SearchDisplay,
                    ItemCreated = s.Document.TimeStamp.CreationTime,
                    ItemId = s.Document.Id,
                    ItemName = s.Document.Name,
                    ItemState = s.Document.Status.State,
                    ItemType = s.Document.DocumentType,
                    PurchasedUserBalance = s.User.Transactions.Balance,
                    PurchasedUserEmail = s.User.Email,
                    PurchasedUserName = s.User.Name,
                    TransactionPrice = s.Price,
                    TransactionTime = s.Created,


                }).ToListAsync(token);

                //User userAlias = null;
                //DocumentTransaction transactionAlias = null;
                //Document documentAlias = null;

                //UserSoldItemsDto userSoldDto = null;

                //var cte = QueryOver.Of(() => documentAlias)
                //    .Where(w => w.User.Id == query.UserId)

                //    .Select(s => s.Id);

                //var userCountry = QueryOver.Of(() => userAlias)
                //    .Where(w => w.Id == query.UserId)
                //    .Select(s => s.Country);

                //var t = _session.QueryOver(() => transactionAlias)
                //    .Inner.JoinAlias(r => r.Document, () => documentAlias, r => r.Id == transactionAlias.Document.Id)
                //    .Inner.JoinAlias(r => r.User, () => userAlias, r => r.Id == transactionAlias.User.Id)
                //    .WithSubquery.WhereProperty(w => w.Document.Id).In(cte);

                //if (!string.IsNullOrEmpty(query.Country))
                //{
                //    t.WithSubquery.WhereValue(query.Country).Eq(userCountry);
                //}

                //return await t.Where(w => w.User.Id != query.UserId)
                //    .SelectList(s =>
                //        s.Select(x => x.Created).WithAlias(() => userSoldDto.TransactionTime)
                //        .Select(x => x.Price).WithAlias(() => userSoldDto.TransactionPrice)
                //        .Select(x => x.Document.Id).WithAlias(() => userSoldDto.ItemId)
                //        .Select(x => documentAlias.Name).WithAlias(() => userSoldDto.ItemName)
                //        .Select(x => documentAlias.TimeStamp.CreationTime).WithAlias(() => userSoldDto.ItemCreated)
                //        .Select(x => documentAlias.Course2.SearchDisplay).WithAlias(() => userSoldDto.ItemCourse)
                //        .Select(x => documentAlias.Status.State).WithAlias(() => userSoldDto.ItemState)
                //        .Select(x => documentAlias.DocumentType).WithAlias(() => userSoldDto.ItemType)
                //        .Select(x => userAlias.Name).WithAlias(() => userSoldDto.PurchasedUserName)
                //        .Select(x => userAlias.Email).WithAlias(() => userSoldDto.PurchasedUserEmail)
                //        .Select(x => userAlias.Transactions.Balance).WithAlias(() => userSoldDto.PurchasedUserBalance)
                //        ).TransformUsing(Transformers.AliasToBean<UserSoldItemsDto>())
                //        .Skip(query.Page * pageSize).Take(pageSize).ListAsync<UserSoldItemsDto>();
            }
        }
    }
}

