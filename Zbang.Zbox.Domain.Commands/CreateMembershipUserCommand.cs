﻿using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommand : CreateUserCommand
    {
        const string ResolveName = "Membership";
        public CreateMembershipUserCommand(Guid membershipId, string email, long? universityId, string firstName,
            string lastName, bool sex, bool marketEmail, string culture, Guid? inviteId, long? boxId, bool isMobile)
            : base(email, universityId, firstName, lastName, sex, marketEmail, culture, inviteId, boxId, isMobile)
        {
            //UniversityName = universityName;
            MembershipUserId = membershipId;
        }

        public Guid MembershipUserId { get; private set; }

        //public string UniversityName  { get; private set; }           

        public override string CommandResolveName
        {
            get { return ResolveName; }
        }
    }
}
