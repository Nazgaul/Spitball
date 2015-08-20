﻿using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Membership";
        public CreateMembershipUserCommand(Guid membershipId, string email, long? universityId, string firstName,
            string lastName, string culture, Guid? inviteId = null, long? boxId = null)
            : base(email, universityId, firstName, lastName,   culture, inviteId, boxId)
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
