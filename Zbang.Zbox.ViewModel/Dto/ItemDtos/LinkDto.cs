using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class LinkDto : ItemDto
    {
        public LinkDto(long id, string name, long ownerId, string userUrl,
            string thumbnail,
             string tabId, int numOfViews, float rate, bool sponsored, string owner, string blobName, DateTime date, string url)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, thumbnail, sponsored, owner, date, userUrl,url)
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
