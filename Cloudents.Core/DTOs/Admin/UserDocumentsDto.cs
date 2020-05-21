using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserDocumentsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Course { get; set; }
        public decimal? Price { get; set; }
        public ItemState State { get; set; }

        public Uri Preview { get; set; }
        public string SiteLink { get; set; }
    }
}