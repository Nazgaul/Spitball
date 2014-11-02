using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommand : CreateUserCommand
    {
        const string ResolveName = "Membership";
        public CreateMembershipUserCommand(Guid membershipId, string email, long? universityId, string firstName, string middleName,
            string lastName, bool sex, bool marketEmail, string culture, Guid? inviteId)
            : base(email, universityId, firstName, middleName, lastName, sex, marketEmail, culture, inviteId)
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
