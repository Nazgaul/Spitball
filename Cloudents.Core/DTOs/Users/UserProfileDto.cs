using System.Collections.Generic;

namespace Cloudents.Core.DTOs.Users
{
    public class UserProfileDto
    {
        public long Id { get; set; }
        public string Name => $"{FirstName} {LastName}";

        public string? Image { get; set; }

        public string? Cover { get; set; }

        public bool Online { get; set; }
        public bool CalendarShared { get; set; }
        public UserTutorProfileDto? Tutor { get; set; }

        public string FirstName { get; set; }
        public string? LastName { get; set; }

        public int Followers { get; set; }

        public bool IsFollowing { get; set; }

      

        public IEnumerable<string>? DocumentCourses { get; set; }
        public IEnumerable<string>? Courses { get; set; }

        //If the user is a tutor and then delete then the first name and the last name stays
        //public bool ShouldSerializeTutor()
        //{
        //    // don't serialize the Manager property if an employee is their own manager
        //    return Tutor?.TutorCountry != null;
        //}
    }
}