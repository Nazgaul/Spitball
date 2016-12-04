using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class Badge
    {
        protected Badge()
        {
            
        }

        public Badge(Guid id, User user, BadgeType type, int progress)
        {
            Id = id;
            User = user;
            Name = type;
            DateTime = DateTime.UtcNow;
            Progress = progress;
            User.BadgeCount++;
        }
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public BadgeType Name { get; set; }
        public DateTime DateTime { get; set; }
        public int Progress { get; set; }
    }
}
