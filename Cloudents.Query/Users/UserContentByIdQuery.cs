﻿using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserContentByIdQuery : IQuery<IEnumerable<UserContentDto>>
    {
        public UserContentByIdQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }

        internal sealed class UserContentByIdQueryHandler : IQueryHandler<UserContentByIdQuery, IEnumerable<UserContentDto>>
        {
            private readonly IStatelessSession _session;

            public UserContentByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserContentDto>> GetAsync(UserContentByIdQuery query, CancellationToken token)
            {
                var documentFuture = _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(UserContentByIdQuery)))
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => new UserDocumentsDto()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Course = s.Course.Name,
                        Type = s.DocumentType.ToString(),
                       // Likes = s.VoteCount,
                        //Price = s.DocumentPrice.Price,
                        //State = s.Status.State,
                        Date = s.TimeStamp.CreationTime,
                        Views = s.Views,
                        Downloads = s.Downloads,
                       // Purchased = s.PurchaseCount ?? 0
                    }).ToFuture<UserContentDto>();

                var documentResult = await documentFuture.GetEnumerableAsync(token);

                return documentResult.OrderByDescending(o => o.Date);
            }
        }
    }
}
