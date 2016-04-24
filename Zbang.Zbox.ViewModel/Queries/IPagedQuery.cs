namespace Zbang.Zbox.ViewModel.Queries
{
    public interface IPagedQuery
    {
        int PageNumber { get; }
        int RowsPerPage { get; }
    }

    public interface IPagedQuery2
    {
        int Top { get; }
        int Skip { get; }
    }
}