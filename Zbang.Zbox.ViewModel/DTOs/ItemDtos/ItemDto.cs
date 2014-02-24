

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public abstract class ItemDto
    {
        protected ItemDto(
            long id,
            string name,
            long ownerId,
            
            string tabid,
            int numOfViews,
            float rate,
            string thumbnail,
            string owner)
        {
            Id = id;
            Name = name;
           
            OwnerId = ownerId;
            TabId = tabid;
            NumOfViews = numOfViews;
            Rate = rate;
            Thumbnail = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(thumbnail);
            Owner = owner;
        }

        public long Id { get; private set; }
        public string Name { get; protected set; }
        public float Rate { get; private set; }
        public long OwnerId { get; private set; }
        public string Owner { get; private set; }
        public abstract string Type { get; }
        public string TabId { get; private set; }

        public int NumOfViews { get; private set; }

        public string Url { get; set; }

        public string Thumbnail { get; private set; }
    }
}
