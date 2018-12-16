using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Domain.Enums;

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


        public static bool ValidateFlagReason(string FlagReason)
        {
            if (string.IsNullOrEmpty(FlagReason))
            {
                return false;
            }

            if (FlagReason.Length > MaxReasonLength)
            {
                return false;
            }
            return true;
        }

        public virtual ItemState State { get; set; }
        public virtual DateTime? DeletedOn { get; set; }
        public virtual string FlagReason { get; set; }

        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }
    }
}