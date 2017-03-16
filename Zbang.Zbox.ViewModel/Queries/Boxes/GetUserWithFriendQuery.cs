namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetUserWithFriendQuery :  IPagedQuery
    {
        public GetUserWithFriendQuery(long friendId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            FriendId = friendId;

        }
        public long FriendId { get; private set; }


        public int PageNumber
        {
            get; }

        public int RowsPerPage
        {
            get; }
    }
}
