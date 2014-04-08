﻿using System;
namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public abstract class ItemDto : IItemDto
    {
        protected ItemDto(
            long id,
            string name,
            long ownerId,

            string tabid,
            int numOfViews,
            float rate,
            string thumbnail,
            bool sponsored,
            string owner,
            DateTime date)
        {
            Id = id;
            Name = name;

            OwnerId = ownerId;
            TabId = tabid;
            NumOfViews = numOfViews;
            Rate = rate;
            Thumbnail = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(thumbnail);
            Owner = owner;
            Sponsored = sponsored;
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public float Rate { get; private set; }
        public long OwnerId { get; set; }
        public string Owner { get; set; }
        public abstract string Type { get; }
        public string TabId { get; private set; }

        public int NumOfViews { get; private set; }

        public string Url { get; set; }
        public string DownloadUrl { get; set; }
        public string UserUrl { get; set; }

        public string Thumbnail { get; private set; }


        public bool Sponsored { get; private set; }

        public DateTime Date { get; set; }

    }
}
