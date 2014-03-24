using System.Runtime.Serialization;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateFacebookUserCommand : CreateUserCommand
    {
        const string resolveName = "Facebook";

        public CreateFacebookUserCommand(long facebookId, string email, string userImage,
            string largeUserImage, long? universityId, string firstName, string middleName, string lastName, bool sex)
            : base(email, universityId, firstName, middleName, lastName, sex)
        {
            FacebookUserId = facebookId;
            UserImage = userImage;
            LargeUserImage = largeUserImage;
        }

        public long FacebookUserId { get; private set; }

        public string UserImage { get; private set; }
        public string LargeUserImage { get; private set; }


        public override string CommandResolveName
        {
            get { return resolveName; }
        }
    }
}
