

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public abstract class ItemDto
    {
        protected ItemDto(long id, string name, long ownerId,
         

            string tabid, int numOfViews, float rate, string owner)
        {
            Id = id;
            Name = name;
           
            OwnerId = ownerId;
            Owner = owner;
            TabId = tabid;
            NumOfViews = numOfViews;
            Rate = rate;
        }

        public long Id { get; private set; }
        public string Name { get; protected set; }
        public abstract string Thumbnail { get; }
        public float Rate { get; private set; }
        public long OwnerId { get; private set; }
        public string Owner { get; private set; }
        public abstract string Type { get; }
        public string TabId { get; private set; }

        public int NumOfViews { get; private set; }

        public string Url { get; set; }
    }
}
