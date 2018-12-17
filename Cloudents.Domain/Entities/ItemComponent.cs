using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Domain.Enums;
using Cloudents.Domain.Interfaces;

namespace Cloudents.Domain.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class ItemComponent
    {
        public ItemComponent()
        {
            Votes = new List<Vote>();
        }
        private const int MaxReasonLength = 255;


        public static bool ValidateFlagReason(string flagReason)
        {
            if (string.IsNullOrEmpty(flagReason))
            {
                return false;
            }

            if (flagReason.Length > MaxReasonLength)
            {
                return false;
            }
            return true;
        }

        public virtual ItemState State { get; set; }
        public virtual DateTime? DeletedOn { get; set; }
        public virtual string FlagReason { get; set; }
        public virtual User FlaggedUser { get; set; }

        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }
    }

    public abstract class ItemObject : ISoftDelete
    {
        public virtual ItemComponent Item { get; set; }
        //public virtual User User { get; set; }

    }
}