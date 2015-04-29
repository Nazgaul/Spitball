using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class 
        ItemMobileDto
    {
        public long Size { get; set; }
        public int NumberOfViews { get; set; }

        public string Url { get; set; }

        public string BoxName { get; set; }

        public DateTime CreationTime { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }

        public string Owner { get; set; }

        public long OwnerId { get; set; }

        public int NumberOfDownloads { get; set; }

        public long Id { get; set; }

        public string Type { get; set; }

        public string ShortUrl { get; set; }
    }
}
