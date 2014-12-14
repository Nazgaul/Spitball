﻿using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateFacebookUserCommand : CreateUserCommand
    {
        const string ResolveName = "Facebook";

        public CreateFacebookUserCommand(long facebookId, string email, string userImage,
            string largeUserImage, long? universityId, string firstName, string middleName, string lastName, bool sex, bool marketEmail, string culture,
            Guid? inviteId, long? boxId)
            : base(email, universityId, firstName, lastName, sex, marketEmail, culture, inviteId, boxId)
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
}
