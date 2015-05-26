using System;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetCommentRepliesQuery : IPagedQuery
    {
        public GetCommentRepliesQuery(long boxId, Guid commentId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            CommentId = commentId;
        }

        public long BoxId { get; private set; }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public Guid CommentId { get; private set; }
    }
}