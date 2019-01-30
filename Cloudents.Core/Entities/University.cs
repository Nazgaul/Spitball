using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class University : Entity<Guid>
    {
        public University(string name) : this()
        {
            Name = name.Replace("+", "-");
            RowDetail = new DomainTimeStamp();
        }

        protected University()
        {

        }

        //public virtual Guid Id { get; protected set; }


        public virtual string Name { get; protected set; }

        /// <summary>
        /// Used as extra synonym to add to university search
        /// </summary>
        public virtual string Extra { get; set; }


        public virtual DomainTimeStamp RowDetail { get; protected set; }
    }
}
