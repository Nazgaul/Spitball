using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserProfileCommand : ICommand
    {
        public UpdateUserProfileCommand(long id, string userName, Uri profileImageUrl,
            Uri largeProfileImageUrl)
        {
            Id = id;
            UserName = userName;
            PicturePath = profileImageUrl;
            LargePicturePath = largeProfileImageUrl;
            //University = university;

        }

        public long Id { get; private set; }
        public string UserName { get; private set; }
        public Uri PicturePath { get; private set; }
        public Uri LargePicturePath { get; private set; }
        //public string University { get; private set; }

        //public long? UniversityId { get; set; }
        //public long? UniversityWrapperId { get;  set; }

    }
}
