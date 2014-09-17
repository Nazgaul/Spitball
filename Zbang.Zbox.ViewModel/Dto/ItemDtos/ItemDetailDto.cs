using System;


namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemDetailDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime UpdateTime { get; set; }
        public string Owner { get; set; }
        public long OwnerId { get; set; }

        public int NumberOfViews { get; set; }

        public int? NumberOfDownloads { get; set; }

        public string Blob { get; set; }
        public float Rate { get;  set; }

        public string BoxName { get; set; }

        public string BoxUrl { get; set; }

        public string PreviousUrl { get; set; }
        public string NextUrl { get; set; }

        public string DownloadUrl { get; set; }
        public string PrintUrl { get; set; }

        public ItemNavigationDto Navigation { get; set; }
    }
}
