using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nHibernate")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "nHibernate")]
    public class AdminTutor
    {
        public AdminTutor(Tutor tutor)
        {
            Tutor = tutor;
        }
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "nhibernate")]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "nhibernate")]
        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected AdminTutor()
        { }

        public virtual Guid Id { get; protected set; }
        public virtual Tutor Tutor { get; protected set; }
    }
}
