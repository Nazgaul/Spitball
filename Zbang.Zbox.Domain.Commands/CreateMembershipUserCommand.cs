using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Membership";
        public CreateMembershipUserCommand(Guid membershipId, string email, long? universityId, string firstName,
            string lastName, string culture, Sex sex, Guid? inviteId = null, long? boxId = null)
            : base(email, universityId, firstName, lastName, culture, inviteId, boxId, sex)
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
