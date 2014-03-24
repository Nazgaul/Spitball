using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserProfileCommand : ICommand
    {
        public UpdateUserProfileCommand(long id,  Uri profileImageUrl,
            Uri largeProfileImageUrl, string firstName, string middleName, string lastName)
        {
            Id = id;
            //UserName = userName;
            PicturePath = profileImageUrl;
            LargePicturePath = largeProfileImageUrl;

            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            //University = university;

        }

        public long Id { get; private set; }
        //public string UserName { get; private set; }
        public Uri PicturePath { get; private set; }
        public Uri LargePicturePath { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MiddleName { get; private set; }
        //public string University { get; private set; }

        //public long? UniversityId { get; set; }
        //public long? UniversityWrapperId { get;  set; }

    }
}
