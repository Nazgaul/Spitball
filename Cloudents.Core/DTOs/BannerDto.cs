using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs
{
    public class BannerDto
    {
        [EntityBind(nameof(BannerBar.Id))]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        [EntityBind(nameof(BannerBar.ExpirationDate))]
        public DateTime ExpirationDate { get; set; }
        public string  Coupon { get; set; }
    }
}
