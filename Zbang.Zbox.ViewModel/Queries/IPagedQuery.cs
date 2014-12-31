namespace Zbang.Zbox.ViewModel.Queries
{
    public interface IPagedQuery
    {
        int PageNumber { get; }
        int RowsPerPage { get; }
    }
}