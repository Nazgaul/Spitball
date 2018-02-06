using System;

namespace Zbang.Zbox.Domain
{
    public class Connection
    {
        // ReSharper disable once UnusedMember.Global nhibernate use
        protected Connection()
        {
            
        }

        internal Connection(string id, User user)
        {
            Id = id;
            User = user;
            LastActivity = DateTimeOffset.UtcNow;
        }
            

        public virtual string Id { get; set; }
        public virtual DateTimeOffset LastActivity { get; set; }

        public virtual User User { get; set; }
    }
}
