﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, ListWithCountDto<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public UserDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<ListWithCountDto<DocumentFeedDto>> GetAsync(UserDocumentsQuery query, CancellationToken token)
        {
            var r = _session.Query<Document>()
                .Fetch(f => f.User)
                .ThenFetch(f => f.University)
                .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                .OrderByDescending(o => o.Boost).ThenByDescending(o => o.TimeStamp.UpdateTime)
                .Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    User = new DocumentUserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.ImageName,
                    },
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Id,
                    Title = s.Name,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.User.University.Name,
                    Snippet = s.Description ?? s.MetaContent,
                    Price = s.Price,
                    Vote = new VoteDto
                    {
                        Votes = s.VoteCount
                    },
                    DocumentType = s.DocumentType ?? DocumentType.Document,
                    Duration = s.Duration,
                    Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument)

                }
                )
                .Take(query.PageSize).Skip(query.Page * query.PageSize).ToFuture();

            var count = _session.Query<Document>().Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok).ToFuture();


            return new ListWithCountDto<DocumentFeedDto>()
            {
                Result = await r.GetEnumerableAsync(token),
                Count = (await count.GetEnumerableAsync(token)).Count()
            };
        }
    }


}