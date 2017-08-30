using System;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Domain
{
    public class UserTimeDetails
    {
        protected UserTimeDetails()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            CreationTime = UpdateTime = DateTime.UtcNow;
            UpdatedUser = CreatedUser = string.Empty;
        }

        public UserTimeDetails(long userId)
        {
            DateTime now = DateTime.UtcNow;
            CreationTime = now;
            UpdateTime = now;
            var platform = ConfigFetcher.Fetch("platform");
            if (string.IsNullOrEmpty(platform))
            {
                platform = string.Empty;
            }

            CreatedUser = UpdatedUser = UpdatedUser = $"{userId} {platform}";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime UpdateTime { get; set; }

        public virtual string CreatedUser { get; protected internal set; }
        public virtual string UpdatedUser { get; protected set; }

        public void UpdateUserTime(long userId)
        {
            DateTime now = DateTime.UtcNow;
            UpdateTime = now;
            var platform =  ConfigFetcher.Fetch("platform");
            if (string.IsNullOrEmpty(platform))
            {
                platform = string.Empty;
            }
            UpdatedUser = $"{userId} {platform}";
        }
    }
}
