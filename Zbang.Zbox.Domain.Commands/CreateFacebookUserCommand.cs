using System;
using System.Collections.Specialized;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateFacebookUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Facebook";

        public CreateFacebookUserCommand(long facebookId, string email,
            string largeUserImage,  string firstName, string lastName, string culture, Sex sex, NameValueCollection parameters,
            Guid? inviteId = null, long? boxId = null, long? universityId = null)
            : base(email, universityId,  firstName, lastName, culture, inviteId, boxId, sex, parameters)
        {
            FacebookUserId = facebookId;
            LargeUserImage = largeUserImage;
        }

        public long FacebookUserId { get; private set; }


        public string LargeUserImage { get; private set; }


        public override string CommandResolveName => ResolveName;
    }
}
