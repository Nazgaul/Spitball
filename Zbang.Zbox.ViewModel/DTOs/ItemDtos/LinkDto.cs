

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long id, string name, long ownerId,
            string thumbnail,
             string tabId, int numOfViews, float rate, bool sponsored, string owner)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, thumbnail,sponsored, owner)
        {
            
        }

        

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
