using System;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetCommentRepliesQuery : IPagedQuery
    {
        public GetCommentRepliesQuery(long boxId, Guid commentId, Guid belowReplyId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            CommentId = commentId;
            BelowReplyId = belowReplyId;
        }

        public long BoxId { get; private set; }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public Guid CommentId { get; private set; }

        public Guid BelowReplyId { get; private set; }
    }
}