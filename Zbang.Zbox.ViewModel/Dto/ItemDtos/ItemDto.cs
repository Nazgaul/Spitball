using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemDto
    {


        public long Id { get; set; }
        public string Name { get; set; }
        public float Rate { get; set; }
        public long OwnerId { get; set; }
        public string Owner { get; set; }
        public Guid? TabId { get; set; }

        public int NumOfViews { get; set; }

        public string Url { get; set; }
        public string DownloadUrl { get; set; }
        public string UserUrl { get; set; }

        public string Thumbnail { get; set; }


        public bool Sponsored { get; set; }

        public DateTime Date { get; set; }


        public int NumOfDownloads { get; private set; }
        public string Description { get; private set; }
        public int CommentsCount { get; private set; }


    }
}
