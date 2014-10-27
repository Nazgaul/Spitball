using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public abstract class ItemWithDetailDto
    {
        protected ItemWithDetailDto(long id, DateTime updateTime, string name,
            string userName, string userImage, long userId,
            int numberOfViews, string blob, float rate, long boxId,
            string boxName, string country, string uniName, string description, string boxUrl)
        {
            Id = id;
            UpdateTime = updateTime;
            Name = name;
            Owner = userName;
            OwnerImg = userImage;
            OwnerUid = userId;
            NumberOfViews = numberOfViews;
            Blob = blob;
            Rate = rate;
            BoxId = boxId;
            BoxName = boxName;
            Country = country ?? string.Empty;
            UniName = uniName;
            Description = description;
            BoxUrl = boxUrl;


        }
        public long Id { get; private set; }
        public string Name { get; protected set; }
        public DateTime UpdateTime { get; private set; }
        public string Owner { get; private set; }
        public string OwnerImg { get; private set; }
        public long OwnerUid { get; private set; }

        public int NumberOfViews { get; private set; }
        public abstract string Type { get; }


        public string Blob { get; set; }
        public float Rate { get; private set; }

        //for check if item is connected to box
        public long BoxId { get; private set; }

        public string BoxName { get; set; }

        public string BoxUrl { get; set; }

        public string Country { get; set; }

        public string UniName { get; set; }

        public string Description { get; set; }

       

    }
}
