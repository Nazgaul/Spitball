using System;
using System.Collections.Specialized;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class CreateUserCommand : ICommandAsync
    {
        protected CreateUserCommand(string emailId, 
            long? universityId, 
            string firstName, 
            string lastName,
             string culture, Guid? inviteId, 
             long? boxId, Sex sex, NameValueCollection parameters)
        {
            Sex = sex;
            Parameters = parameters;
            BoxId = boxId;
            InviteId = inviteId;
            Culture = culture;
            Email = emailId;
            UniversityId = universityId;
            FirstName = firstName;
            LastName = lastName;
           
        }

        public string Email { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        

        public long? UniversityId { get; private set; }

        public long? BoxId { get; private set; }

        public NameValueCollection Parameters { get; private set; }

        public string Culture { get; private set; }

        public abstract string CommandResolveName { get; }

        public Guid? InviteId { get; private set; }

        public Sex Sex { get; private set; }

    }
}
