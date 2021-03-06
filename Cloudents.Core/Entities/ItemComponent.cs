﻿using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;

namespace Cloudents.Core.Entities
{
    public sealed class ItemStatus : ValueObject
    {
        public ItemState State { get; }
        public DateTime? DeletedOn { get; }

        //public string? FlagReason { get; }
        //public User? FlaggedUser { get; }


       // private const int MaxReasonLength = 255;


        //private static bool ValidateFlagReason(string flagReason)
        //{
        //    if (string.IsNullOrEmpty(flagReason))
        //    {
        //        return false;
        //    }

        //    if (flagReason.Length > MaxReasonLength)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

       // public const string TooManyVotesReason = "Too many down vote";


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return State;
        }

        private ItemStatus()
        {

        }
        private ItemStatus(ItemState state, DateTime? deletedOn) : this()
        {
            State = state;
            DeletedOn = deletedOn;
           // FlagReason = flagReason;
           // FlaggedUser = flaggedUser;
        }

        public static readonly ItemStatus Public = new ItemStatus(ItemState.Ok, null) ;
        public static readonly ItemStatus Pending = new ItemStatus(ItemState.Pending, null);
        //public static readonly ItemStatus Flagged = new ItemStatus(ItemState.Flagged, null, null, null);
        //public static readonly ItemStatus FlagStatus = new ItemStatus(ItemState.Deleted, null, null, null);

        //public ItemStatus Flag(string reason, User user)
        //{
        //    if (!ValidateFlagReason(reason))
        //    {
        //        throw new ArgumentException("reason is too long");
        //    }

        //    if (this != Public)
        //    {
        //        throw new ArgumentException("Not Flagged state");

        //    }
        //    return new ItemStatus(ItemState.Flagged, null, reason, user);
        //}
        //public static ItemStatus GetInitState(User user)
        //{
        //    var country = user.SbCountry ?? Country.UnitedStates;
        //    if (country == Country.India)
        //    {
        //        return Pending;
        //    }
        //    return Public;
        //}

        public static ItemStatus Delete()
        {
            return new ItemStatus(ItemState.Deleted, DateTime.UtcNow);
        }


    }
}