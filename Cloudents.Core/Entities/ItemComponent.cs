using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities
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

        public virtual ItemState State { get; protected set; }
        public virtual DateTime? DeletedOn { get; protected set; }
        public virtual string FlagReason { get; set; }
        public virtual User FlaggedUser { get; set; }

        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }

        public virtual void ChangeState(ItemState state)
        {
            if (state == ItemState.Deleted)
            {
                DeletedOn = DateTime.UtcNow;
            }
        }
    }

    public abstract class ItemObject : AggregateRoot , ISoftDelete
    {
        public virtual ItemComponent Item { get; set; }

        //public virtual User User { get; set; }

    }

    //public class ItemDelete : ValueObject
    //{
    //    public virtual ItemState State { get; protected set; }
    //    public virtual DateTime? DeletedOn { get; protected set; }
    //    protected override IEnumerable<object> GetEqualityComponents()
    //    {
    //        yield return State;
    //        yield return DeletedOn;
    //    }

    //    public virtual void ChangeState(ItemState state)
    //    {

    //    }
    //}

    public abstract class AggregateRoot
    {
        public AggregateRoot()
        {
            Events = new List<IEvent>();
        }
        public virtual IList<IEvent> Events { get; set; }
    }


    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                throw new ArgumentException($"Invalid comparison of Value Objects of different types: {GetType()} and {obj.GetType()}");

            var valueObject = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        public static bool operator ==(ValueObject a, ValueObject b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject a, ValueObject b)
        {
            return !(a == b);
        }
    }
}