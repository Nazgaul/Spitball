using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Nhibernate proxy")]
    public class Audit
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate proxy")]
        public Audit(ICommand command)
        {
            Command = command;
            DateTime = DateTime.UtcNow;
        }

        protected Audit()
        {

        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Nhibernate generate this")]
        public virtual Guid Id { get; protected set; }

        public virtual ICommand Command { get; protected set; }

        public virtual DateTime DateTime { get; protected set; }
    }
}