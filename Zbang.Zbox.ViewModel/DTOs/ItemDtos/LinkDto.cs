

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long id, string name, long ownerId,
            string thumbnail,
             string tabId, int numOfViews, float rate, bool sponsored, string owner, string blobName)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, thumbnail, sponsored, owner, null)
        {
            LinkUrl = blobName;
        }

        public string LinkUrl { get; set; }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
