using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public sealed class ItemState2 : ValueObject
    {
        public ItemState State { get; }
        public DateTime? DeletedOn { get; }

        public string FlagReason { get; }
        public User FlaggedUser { get; }


        private const int MaxReasonLength = 255;


        private static bool ValidateFlagReason(string flagReason)
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

        public const string TooManyVotesReason = "Too many down vote";


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return State;
        }

        private ItemState2()
        {

        }
        private ItemState2(ItemState state, DateTime? deletedOn, string flagReason, User flaggedUser) : this()
        {
            State = state;
            DeletedOn = deletedOn;
            FlagReason = flagReason;
            FlaggedUser = flaggedUser;
        }

        public ItemState2 Flag(string reason, User user)
        {
            if (!ValidateFlagReason(reason))
            {
                throw new ArgumentException("reason is too long");
            }
            return new ItemState2(ItemState.Flagged, null, reason, user);
        }

        public static ItemState2 GetInitState(User user)
        {
            if (user.Score < Privileges.Post)
            {
                return Pending();
            }

            return Public();
        }
    

        public static ItemState2 Public()
        {
            return new ItemState2(ItemState.Ok, null, null, null);
        }

        public static ItemState2 Pending()
        {
            return new ItemState2(ItemState.Pending, null, null, null);
        }

        public static ItemState2 Delete()
        {
            return new ItemState2(ItemState.Deleted, DateTime.UtcNow, null, null);
        }


        public static implicit operator ItemState(ItemState2 state)
        {
            return state.State;
        }
    }

    public abstract class ItemObject : AggregateRoot, ISoftDelete
    {
        protected ItemObject()
        {
            State = ItemState2.Pending();// ItemState.Pending;
            Votes = new List<Vote>();
        }


        public abstract void DeleteAssociation();

        public virtual ItemState2 State { get; private set; }
        //public virtual DateTime? DeletedOn { get; private set; }


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
                case ItemState.Pending:
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

            State = ItemState2.Public();
            return true;
        }

        public virtual void Delete()
        {
            if (State == ItemState.Deleted)
            {
                return;
            }

            State = ItemState2.Delete();
            DeleteAssociation();
        }


        public virtual bool Flag(string reason, User user)
        {
            if (State == ItemState.Flagged)
            {
                return false;
            }
            State = State.Flag(reason,user);
            return true;
        }




        public virtual ICollection<Vote> Votes { get; protected set; }

        public virtual int VoteCount { get; set; }

    }



    public abstract class AggregateRoot
    {

        private readonly List<IEvent> _domainEvents = new List<IEvent>();
        public virtual IReadOnlyList<IEvent> Events => _domainEvents;
        //protected AggregateRoot()
        //{
        //    Events = new List<IEvent>();
        //}
        //public virtual IList<IEvent> Events { get; set; }

        protected virtual void AddEvent(IEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }
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