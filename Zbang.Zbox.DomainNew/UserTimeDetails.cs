using System;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Domain
{
    public class UserTimeDetails
    {
        protected UserTimeDetails()
        {
            DateTime now = DateTime.UtcNow;
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            CreationTime = now;

            UpdateTime = now;
            CreatedUser = string.Empty;
            UpdatedUser = string.Empty;
        }

        public UserTimeDetails(string userName/*, Platform platform = Platform.Default*/)
        {
            DateTime now = DateTime.UtcNow;
            CreationTime = now;
            UpdateTime = now;
            var platform = ConfigFetcher.Fetch("platform");
            if (string.IsNullOrEmpty(platform))
            {
                platform = string.Empty;
            }

            CreatedUser = UpdatedUser = UpdatedUser = $"{userName} {platform}";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime UpdateTime { get; set; }

        public virtual string CreatedUser { get; protected internal set; }
        public virtual string UpdatedUser { get; protected set; }

        public void UpdateUserTime(string userName/*, Platform platform = Platform.Default*/)
        {
            DateTime now = DateTime.UtcNow;
            UpdateTime = now;
            var platform =  ConfigFetcher.Fetch("platform");
            if (string.IsNullOrEmpty(platform))
            {
                platform = string.Empty;
            }
            UpdatedUser = $"{userName} {platform}";
        }
    }
}
