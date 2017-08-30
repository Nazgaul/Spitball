using System;
using System.Collections.Specialized;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateMembershipUserCommand : CreateUserCommand
    {
        public const string ResolveName = "Membership";
        public CreateMembershipUserCommand(Guid membershipId, string email, string firstName,
            string lastName, string culture, Sex sex, NameValueCollection parameters, Guid? inviteId = null, long? boxId = null, long? universityId = null)
            : base(email, universityId, firstName, lastName, culture, inviteId, boxId, sex, parameters)
        {
            //UniversityName = universityName;
            MembershipUserId = membershipId;
        }

        public Guid MembershipUserId { get; private set; }
        
        public override string CommandResolveName => ResolveName;
    }
}
