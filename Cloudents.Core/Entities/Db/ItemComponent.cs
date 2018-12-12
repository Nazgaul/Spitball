using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class ItemComponent
    {
        public ItemComponent()
        {
            Votes = new List<Vote>();
        }
        public virtual ItemState State { get; set; }
        public virtual DateTime? DeletedOn { get; set; }
        public virtual string FlagReason { get; set; }

        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }
    }
}