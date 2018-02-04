using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemDetailDto
    {
        private DateTime _date;
        public string Name { get; set; }

        public DateTime Date
        {
            get { return DateTime.SpecifyKind(_date, DateTimeKind.Utc); }
            set { _date = value; }
        }

        public string Owner { get; set; }
        public long OwnerId { get; set; }

        public int OwnerScore { get; set; }
        public int OwnerBadges { get; set; }

        public int NumberOfViews { get; set; }

        public int? NumberOfDownloads { get; set; }

        public string Blob { get; set; }

        public int Like { get; set; }
        public int Likes { get; set; }
        public string Type { get; set; }

        //public string BoxUrl { get; set; }

        //public string DownloadUrl { get; set; }
        //public string PrintUrl { get; set; }

        public ItemNavigationDto Navigation { get; set; }

        public UserRelationshipType UserType { get; set; }

        //public IEnumerable<ActivityDtos.AnnotationDto> Comments { get; set; }

        public string ShortUrl { get; set; }
    }
}
