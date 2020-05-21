﻿using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserDocumentsQuery : IQuery<ListWithCountDto<DocumentFeedDto>>
    {
        public UserDocumentsQuery(long id, int page, int pageSize, DocumentType? documentType, string? course, long userId)
        {
            Id = id;
            Page = page;
            PageSize = pageSize;
            DocumentType = documentType;
            Course = course;
            UserId = userId;
        }

        private long Id { get; }
        private int Page { get; }
        private int PageSize { get; }

        private DocumentType? DocumentType { get; }
        private string? Course { get; }

        private long UserId { get; }

        internal sealed class UserDocumentsQueryHandler : IQueryHandler<UserDocumentsQuery, ListWithCountDto<DocumentFeedDto>>
        {
            private readonly IStatelessSession _session;

            public UserDocumentsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<ListWithCountDto<DocumentFeedDto>> GetAsync(UserDocumentsQuery query, CancellationToken token)
            {
                var r = _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(UserDocumentsQuery)))
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok);
                if (query.DocumentType != null)
                {
                    r = r.Where(w => w.DocumentType == query.DocumentType);
                }
                if (!string.IsNullOrEmpty(query.Course))
                {
                    r = r.Where(w => w.Course.Id == query.Course);
                }
                var count = r;
                r = r.OrderByDescending(o => o.Boost).ThenByDescending(o => o.TimeStamp.UpdateTime);
                var result = r.Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Id,
                    Title = s.Name,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    Snippet = s.Description ?? s.MetaContent,
                    Price = s.DocumentPrice.Price,
                    Vote = new VoteDto
                    {
                        Votes = s.VoteCount
                    },
                    PriceType = s.DocumentPrice.Type ?? PriceType.Free,
                    DocumentType = s.DocumentType,
                    Duration = s.Duration,
                    Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument)
                }).Take(query.PageSize).Skip(query.Page * query.PageSize).ToFuture();

                var countFuture = count
                .GroupBy(g => 1)
                .Select(s => s.Count()).ToFutureValue();


                IFutureValue<bool?>? scribedQueryFuture = null;
                if (query.UserId > 0)
                {
                    scribedQueryFuture = _session.Query<Follow>()
                       .Where(w => w.Follower.Id == query.UserId)
                       .Where(w => w.User.Id == query.Id)
                       .Select(s => s.Subscriber).ToFutureValue();

                }


                var futureResult = await result.GetEnumerableAsync(token);
                var isSubscribed = scribedQueryFuture?.Value ?? query.UserId == query.Id;

                if (isSubscribed)
                {
                    futureResult = futureResult.Select(s =>
                    {
                        s.PriceType = PriceType.Free;
                        return s;
                    });
                }
                return new ListWithCountDto<DocumentFeedDto>()
                {
                    Result = futureResult,
                    Count = countFuture.Value
                };
            }
        }
    }
}
