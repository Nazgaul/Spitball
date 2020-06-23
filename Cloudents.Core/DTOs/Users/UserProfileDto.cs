using System;
using System.Collections.Generic;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class UserProfileDto
    {
        public long Id { get; set; }
        public string Name => $"{FirstName} {LastName}";

        public string? Image { get; set; }

        public string? Cover { get; set; }

        //public bool Online { get; set; }
        public bool CalendarShared { get; set; }

        public string FirstName { get; set; }
        public string? LastName { get; set; }

        public int Followers { get; set; }

        public bool IsFollowing { get; set; }


        public Country? TutorCountry { get; set; }


        public int ReviewCount { get; set; }

        //public bool HasCoupon { get; set; }

        //  public decimal? CouponValue { get; set; }
        //  public CouponType? CouponType { get; set; }

        public string? Bio { get; set; }

        // public IEnumerable<string>? Subjects { get; set; }


        //public int ContentCount { get; set; }
        public int ContentCount { get; set; }

        public Money? SubscriptionPrice { get; set; }

        public bool IsSubscriber { get; set; }

        public string? Description { get; set; }

        public string? Paragraph =>
            "International accredited online yoga teacher. \r\nI have tougth more than 1,000 hours of yoga.  A solid foundation for teaching yoga and an in-depth exploration of how to deepen your own yoga personal practice … read more. ";


        [NonSerialized] public long SessionTaughtTicks;

        public long HoursTaught => SessionTaughtTicks / TimeSpan.TicksPerHour;
        public IEnumerable<string>? DocumentCourses { get; set; }
        //public IEnumerable<string>? Courses { get; set; }

        //If the user is a tutor and then delete then the first name and the last name stays
        //public bool ShouldSerializeTutor()
        //{
        //    // don't serialize the Manager property if an employee is their own manager
        //    return Tutor?.TutorCountry != null;
        //}
    }
}