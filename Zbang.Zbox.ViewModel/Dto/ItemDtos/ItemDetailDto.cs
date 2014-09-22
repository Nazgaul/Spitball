using System;
using System.Collections.Generic;


namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemDetailDto
    {
        private DateTime m_Date;
        public string Name { get; set; }

        public DateTime UpdateTime
        {
            get { return DateTime.SpecifyKind(m_Date, DateTimeKind.Utc); }
            set { m_Date = value; }
        }

        public string Owner { get; set; }
        public long OwnerId { get; set; }

        public int NumberOfViews { get; set; }

        public int? NumberOfDownloads { get; set; }

        public string Blob { get; set; }
        public int Rate { get;  set; }

        public string BoxName { get; set; }

        public string BoxUrl { get; set; }

        public string PreviousUrl { get; set; }
        public string NextUrl { get; set; }

        public string DownloadUrl { get; set; }
        public string PrintUrl { get; set; }

        public ItemNavigationDto Navigation { get; set; }

        public IEnumerable<ActivityDtos.AnnotationDto> Comments { get; set; }
    }
}
