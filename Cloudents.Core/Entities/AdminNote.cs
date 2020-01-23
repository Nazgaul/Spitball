using System;

namespace Cloudents.Core.Entities
{
    public class AdminNote : Entity<Guid>
    {
        public AdminNote(string text, User user, AdminUser adminUser) : this()
        {
            Text = text;
            User = user;
            AdminUser = adminUser;
        }
        protected AdminNote()
        {
            TimeStamp = new DomainTimeStamp();
        }
        public virtual string  Text { get; protected set; }
        public virtual DomainTimeStamp TimeStamp { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual AdminUser AdminUser { get; protected set; }
    }
}
