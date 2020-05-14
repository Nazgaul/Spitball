using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class UserProfileDto
    {
        [EntityBind(nameof(User.Id))]
        public long Id { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string Name { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string? Image { get; set; }

        [EntityBind(nameof(User.CoverImage))]
        public string? Cover { get; set; }

        [EntityBind(nameof(User.Online))]
        public bool Online { get; set; }
        [EntityBind(nameof(GoogleTokens))]
        public bool CalendarShared { get; set; }
        [EntityBind(nameof(User.Tutor))]
        public UserTutorProfileDto? Tutor { get; set; }

        [EntityBind(nameof(User.FirstName))]
        public string FirstName { get; set; }
        [EntityBind(nameof(User.LastName))]
        public string? LastName { get; set; }

        [EntityBind(nameof(User.Followers))]
        public int Followers { get; set; }

        public bool IsFollowing { get; set; }

      

        public IEnumerable<string>? DocumentCourses { get; set; }
        public IEnumerable<string>? Courses { get; set; }

        //If the user is a tutor and then delete then the first name and the last name stays
        public bool ShouldSerializeTutor()
        {
            // don't serialize the Manager property if an employee is their own manager
            return Tutor?.TutorCountry != null;
        }
    }
}