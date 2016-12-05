using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetCommentRepliesQuery : IPagedQuery, IQueryCache
    {
        public GetCommentRepliesQuery(long boxId, Guid commentId, Guid belowReplyId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            CommentId = commentId;
            BelowReplyId = belowReplyId;
        }

        public long BoxId { get; }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public Guid CommentId { get; }

        public Guid BelowReplyId { get; }
        public string CacheKey  => $"{CommentId}_{BelowReplyId}_{PageNumber}_{RowsPerPage}";
        public string CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
    }
}