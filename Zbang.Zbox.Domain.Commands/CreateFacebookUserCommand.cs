using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateFacebookUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Facebook";

        public CreateFacebookUserCommand(long facebookId, string email, string userImage,
            string largeUserImage, long? universityId, string firstName, string middleName, string lastName, string culture,
            Guid? inviteId = null, long? boxId = null)
            : base(email, universityId, firstName, lastName,  culture, inviteId, boxId)
        {
            FacebookUserId = facebookId;
            UserImage = userImage;
            LargeUserImage = largeUserImage;
            MiddleName = middleName;
        }

        public long FacebookUserId { get; private set; }

        public string MiddleName { get; private set; }

        public string UserImage { get; private set; }
        public string LargeUserImage { get; private set; }


        public override string CommandResolveName
        {
            get { return ResolveName; }
        }
    }

    public class CreateGoogleUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Google";
        public CreateGoogleUserCommand(string email, string googleId, string image, long? universityId, string firstName, string lastName, string culture, Guid? inviteId = null, long? boxId = null)
            : base(email, universityId, firstName, lastName,  culture, inviteId, boxId)
        {
           GoogleId = googleId;
           Image = image;
        }

        public string GoogleId { get; private set; }
        public string Image { get; private set; }
        public override string CommandResolveName
        {
            get { return ResolveName; }
        }
    }

}
