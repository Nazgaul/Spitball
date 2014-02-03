

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class LinkDto : ItemDto
    {
        private string m_Thumbnail;
        public LinkDto(long id, string name, long ownerId,
             string tabId, int numOfViews, float rate, string owner)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, owner)
        {
            m_Thumbnail = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailLinkUrl();
        }

        public override string Thumbnail
        {
            get
            {
                return m_Thumbnail;
            }

        }

        public override string Type
        {
            get { return "Link"; }
        }
    }
}
