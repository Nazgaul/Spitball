using Cloudents.Core.Attributes;
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
        [EntityBind(nameof(User.University))]
        public string University { get; set; }
        [EntityBind(nameof(User.Country))]
        public string Country { get; set; }
      
     

        public int ReferredCount { get; set; }
        [EntityBind(nameof(User.Balance))]
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Joined { get; set; }
        public bool WasSuspended { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        [EntityBind(nameof(User.LastOnline))]
        public DateTime? LastOnline { get; set; }
        public ItemState? TutorState { get; set; }
        [EntityBind(nameof(User.LockoutReason))]
        public string LockoutReason { get; set; }
        public bool PaymentExists { get; set; }

        public decimal? TutorPrice { get; set; }
    }
}
