using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemDto
    {


        public long Id { get; set; }
        public string Name { get; set; }
        public int Likes { get; set; }
        public long OwnerId { get; set; }
        public string Owner { get; set; }
        public Guid? TabId { get; set; }

        public int NumOfViews { get; set; }

        public string Url { get; set; }
        //public string DownloadUrl { get; set; }
        //public string UserUrl { get; set; }

        public DateTime Date { get; set; }


        public int NumOfDownloads { get;  set; }
        public string Description { get;  set; }

        public long? BoxId { get; set; }

        public string Type { get; set; }

        public string Source { get; set; }


    }
}
