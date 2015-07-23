using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries
{


    public class GetUserFriendsQuery : IUserQuery,IPagedQuery
    {
        public GetUserFriendsQuery(long userId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            UserId = userId;
        }

        public long UserId { get; private set; }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }
    }
}
