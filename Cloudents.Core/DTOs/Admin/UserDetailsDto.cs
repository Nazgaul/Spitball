using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserDetailsDto
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Country { get; set; }
      
     

        public int ReferredCount { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Joined { get; set; }
        public bool WasSuspended { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? LastOnline { get; set; }
        public ItemState? TutorState { get; set; }
        public string? LockoutReason { get; set; }
        public bool PaymentExists { get; set; }
        public bool CalendarExists { get; set; }
       // public decimal? TutorPrice { get; set; }

        public bool HasSubscription { get; set; }


        public string? ProfileUrl { get; set; }
    }
}
