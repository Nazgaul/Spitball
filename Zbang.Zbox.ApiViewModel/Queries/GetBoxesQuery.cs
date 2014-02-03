
namespace Zbang.Zbox.ApiViewModel.Queries
{
    public class GetBoxesQuery : GetBoxesQueryBase
    {
        public GetBoxesQuery(long Id)
            : base(Id, "APIGetBoxes")
        {
            
        }
        
        public override string QueryName
        {
            get { return m_QueryName; }
        }
    }   

   
}
