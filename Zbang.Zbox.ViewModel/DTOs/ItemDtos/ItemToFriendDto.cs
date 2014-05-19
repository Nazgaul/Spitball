using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class ItemToFriendDto
    {
        public ItemToFriendDto(long id, string image, double rate, int numOfViews, string name, long boxId, string boxName,
            string universityName)
        {
            var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
            Id = id;
            if (string.IsNullOrEmpty(image))
            {
                Image = blobProvider.GetThumbnailLinkUrl();
               // Image = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailLinkUrl();
            }
            else
            {
                Image = blobProvider.GetThumbnailUrl(image);
               // Image = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(image);
            }
            Rate = rate;
            NumOfViews = numOfViews;
            Name = name;
            BoxId = boxId;
            BoxName = boxName;
            UniversityName = universityName;
        }
        public long Id { get; set; }
        public string Image { get; set; }
        public double Rate { get; set; }
        public int NumOfViews { get; set; }
        public string Name { get; set; }
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }

    }
}
