using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserDetailsDto
    {
        
        [EntityBind(nameof(RegularUser.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(RegularUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(RegularUser.Email))]
        public string Email { get; set; }
        [EntityBind(nameof(RegularUser.PhoneNumber))]
        public string PhoneNumber { get; set; }
        [EntityBind(nameof(RegularUser.University))]
        public string University { get; set; }
        [EntityBind(nameof(RegularUser.Country))]
        public string Country { get; set; }
        [EntityBind(nameof(RegularUser.Score))]
        public int Score { get; set; }
        //[EntityBind(nameof(RegularUser.FraudScore))]
        //public int FraudScore { get; set; }
       
        public int ReferredCount { get; set; }
        [EntityBind(nameof(RegularUser.Balance))]
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Joined { get; set; }
        public bool WasSuspended { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        [EntityBind(nameof(RegularUser.LastOnline))]
        public DateTime? LastOnline { get; set; }
        public ItemState? TutorState{ get; set; }
        [EntityBind(nameof(RegularUser.LockoutReason))]
        public string LockoutReason { get; set; }
    }
}
