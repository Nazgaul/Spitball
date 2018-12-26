using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{

   
    public abstract class ItemObject : AggregateRoot, ISoftDelete
    {
        protected ItemObject()
        {
            State = ItemState.Pending;
            Votes = new List<Vote>();
        }
        //protected ItemObject(RegularUser user)
        //{
        //    State = ItemState.Pending;
        //    if (user.Score >= Privileges.Post)
        //    {
        //        MakePublic();
        //    }
        //     //Privileges.GetItemState(user.Score);
        //}

        //protected ItemObject()
        //{

        //}

        public abstract void DeleteAssociation();

        public virtual ItemState State { get; private set; }
        public virtual DateTime? DeletedOn { get; private set; }


        public virtual void ChangeState(ItemState state)
        {
            switch (state)
            {
                case ItemState.Ok:
                    MakePublic();
                    break;
                case ItemState.Deleted:
                    Delete();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
        }
        
        public virtual bool MakePublic()
        {
            if (State == ItemState.Ok)
            {
                return false;
            }
            State = ItemState.Ok;
            FlagReason = null;
            FlaggedUser = null;
            return true;
        }

        public virtual bool Delete()
        {
            if (State == ItemState.Deleted)
            {
                return false;
            }
            State = ItemState.Deleted;
            DeletedOn = DateTime.UtcNow;
            DeleteAssociation();
            return true;
        }


        public virtual bool Flag(string reason, User user)
        {
            if (State == ItemState.Flagged)
            {
                return false;
            }
            State = ItemState.Flagged;
            FlagReason = reason;
            FlaggedUser = user;
            return true;
        }


        public virtual string FlagReason { get; private set; }
        public virtual User FlaggedUser { get; private set; }


       
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




        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }

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