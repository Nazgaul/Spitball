using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nHibernate")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate")]
    public class AdminNote : Entity<Guid>
    {
        public AdminNote(string text, User user, AdminUser adminUser) : this()
        {
            Text = text;
            User = user;
            AdminUser = adminUser;

        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "nhibernate")]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "nhibernate")]
        [SuppressMessage("NullChecker", "CS8618", Justification = "nhibernate")]
        protected AdminNote()
        {
            TimeStamp ??= new DomainTimeStamp();
        }
        public virtual string Text { get; protected set; }
        public virtual DomainTimeStamp TimeStamp { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual AdminUser AdminUser { get; protected set; }
    }
}
