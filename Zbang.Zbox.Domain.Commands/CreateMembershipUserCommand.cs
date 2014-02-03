using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommand : CreateUserCommand
    {
        const string ResolveName = "Membership";
        public CreateMembershipUserCommand(Guid membershipId, string email, string userName, long? universityId)
            : base(email, userName, universityId)
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
