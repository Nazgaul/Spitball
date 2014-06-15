
namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public abstract class GetBoxesQueryBase : QueryBase
    {
        protected const string PageQueryPrefix = "Page";
        protected string m_QueryName;

        protected GetBoxesQueryBase(long id, string boxesQueryName, int pageNumber)
            : base(id)
        {
            m_QueryName = boxesQueryName;
            PageNumber = pageNumber;
        }
        public abstract string QueryName { get; }

        public int PageNumber { get; private set; }
    }
}
