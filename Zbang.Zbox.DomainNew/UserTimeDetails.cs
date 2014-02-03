using System;

namespace Zbang.Zbox.Domain
{
    public class UserTimeDetails
    {
        protected UserTimeDetails()
        {
            DateTime now = DateTime.UtcNow;
            CreationTime = now;
            UpdateTime = now;
            CreatedUser = string.Empty;
            UpdatedUser = string.Empty;
        }

        public UserTimeDetails(string user)
        {
            DateTime now = DateTime.UtcNow;
            CreationTime = now;
            UpdateTime = now;
            CreatedUser = UpdatedUser = user;

        }

        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime UpdateTime { get; set; }

        public virtual string CreatedUser { get; set; }
        public virtual string UpdatedUser { get; set; }

        public void UpdateUserTime(string userName)
        {
            DateTime now = DateTime.UtcNow;
            UpdateTime = now;
            UpdatedUser = userName;
        }
    }
}
