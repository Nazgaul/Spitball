﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserDetailsDto
    {

        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(User.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(User.PhoneNumber))]
        public string PhoneNumber { get; set; }

        [EntityBind(nameof(User.Country))]
        public string Country { get; set; }
      
     

        public int ReferredCount { get; set; }
        [EntityBind(nameof(User.Balance))]
        public decimal Balance { get; set; }
        [EntityBind(nameof(User.Online))]
        public bool IsActive { get; set; }
        [EntityBind(nameof(User.Created))]
        public DateTime? Joined { get; set; }
        public bool WasSuspended { get; set; }
        [EntityBind(nameof(User.PhoneNumberConfirmed))]
        public bool PhoneNumberConfirmed { get; set; }
        [EntityBind(nameof(User.EmailConfirmed))]
        public bool EmailConfirmed { get; set; }
        [EntityBind(nameof(User.LastOnline))]
        public DateTime? LastOnline { get; set; }
        [EntityBind(nameof(User.Tutor.State))]
        public ItemState? TutorState { get; set; }
        [EntityBind(nameof(User.LockoutReason))]
        public string LockoutReason { get; set; }
        [EntityBind(nameof(User.PaymentExists))]
        public bool PaymentExists { get; set; }
        public bool CalendarExists { get; set; }
        public decimal? TutorPrice { get; set; }
        public UserType? UserType { get; set; }


        public string ProfileUrl { get; set; }
    }
}
