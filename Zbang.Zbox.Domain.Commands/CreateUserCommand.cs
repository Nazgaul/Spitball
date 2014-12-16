using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class CreateUserCommand : ICommandAsync
    {
        protected CreateUserCommand(string emailId, long? universityId, string firstName,  string lastName, bool sex,
            bool marketEmail, string culture, Guid? inviteId, long? boxId)
        {
            BoxId = boxId;
            InviteId = inviteId;
            Culture = culture;
            Email = emailId;
            if (universityId.HasValue && universityId.Value == 19878)
            {
                universityId = null;
                
            }
            UniversityId = universityId;
            FirstName = firstName;
            LastName = lastName;
            Sex = sex;
            MarketEmail = marketEmail;
        }

        public string Email { get; private set; }

        //public string UserName { get; private set; }
        public string FirstName { get; private set; }
        
        public string LastName { get; private set; }
        public bool Sex { get; private set; }

        public bool MarketEmail { get; private set; }

        public long?  UniversityId { get; private set; }

        public long? BoxId { get; private set; }    

        public string Culture { get; private set; }

        public abstract string CommandResolveName { get; }

        public Guid? InviteId { get; private set; }
    }
}
