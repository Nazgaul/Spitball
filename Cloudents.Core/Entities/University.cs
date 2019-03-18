using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class University : Entity<Guid>
    {
        public University(string name, string country) : this()
        {
            Name = name.Replace("+", "-");
            Country = country;
            RowDetail = new DomainTimeStamp();
        }

        protected University()
        {

        }

        public virtual void Approve()
        {
            //TODO: maybe put an event to that
            if (State == ItemState.Pending)
            {
                State = ItemState.Ok;
            }
        }

        //public virtual Guid Id { get; protected set; }


        public virtual string Name { get; protected set; }

        /// <summary>
        /// Used as extra synonym to add to university search
        /// </summary>
        public virtual string Extra { get; set; }

        public virtual string Country { get; protected set; }

        public virtual DomainTimeStamp RowDetail { get; protected set; }

        public virtual IList<Document> Documents { get; set; }
        public virtual IList<Question> Questions { get; set; }
        public virtual IList<User> Users { get; set; }
        public virtual ItemState State { get; protected set; }
    }
}
