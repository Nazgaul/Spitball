

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long id, string name, long ownerId,
            string thumbnail,
             string tabId, int numOfViews, float rate, string owner)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, owner,thumbnail)
        {
        }

       

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
