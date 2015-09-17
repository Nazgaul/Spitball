using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateFacebookUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Facebook";

        public CreateFacebookUserCommand(long facebookId, string email,
            string largeUserImage, long? universityId, string firstName, string lastName, string culture,
            Guid? inviteId = null, long? boxId = null)
            : base(email, universityId, firstName, lastName,  culture, inviteId, boxId)
        {
            FacebookUserId = facebookId;
            LargeUserImage = largeUserImage;
        }

        public long FacebookUserId { get; private set; }


        public string LargeUserImage { get; private set; }


        public override string CommandResolveName
        {
            get { return ResolveName; }
        }
    }
}
