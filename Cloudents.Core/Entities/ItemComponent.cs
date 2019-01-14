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