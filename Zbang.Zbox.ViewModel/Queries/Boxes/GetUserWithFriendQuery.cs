namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetUserWithFriendQuery :  IPagedQuery
    {
        public GetUserWithFriendQuery(/*long userId,*/ long friendId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            FriendId = friendId;
            //UserId = userId;

        }
        public long FriendId { get; private set; }

        //public long UserId
        //{
        //    get;
        //    private set;
        //}

        public int PageNumber
        {
            get;
            private set;
        }

        public int RowsPerPage
        {
            get;
            private set;
        }
    }
}
