using System;

namespace Cloudents.Core.Entities
{
    public class BannerBar : Entity<Guid>
    {
        public virtual string EnTitle { get; protected set; }
        public virtual string EnSubTitle { get; protected set; }
        public virtual string HeTitle { get; protected set; }
        public virtual string HeSubTitle { get; protected set; }
        public virtual string EnInTitle { get; protected set; }
        public virtual string EnInSubTitle { get; protected set; }
        public virtual DateTime ExpirationDate { get; protected set; }
        public virtual Coupon Coupon { get; set; }
    }
}
