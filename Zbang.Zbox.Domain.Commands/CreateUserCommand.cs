using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class CreateUserCommand : ICommand
    {
        protected CreateUserCommand(string emailId, long? universityId, string firstName, string middleName, string lastName, bool sex,
            bool marketEmail, string culture)
        {
            Culture = culture;
            Email = emailId;
            if (universityId.HasValue && universityId.Value == 19878)
            {
                universityId = null;
                
            }
            UniversityId = universityId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Sex = sex;
            MarketEmail = marketEmail;
        }

        public string Email { get; private set; }

        //public string UserName { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public bool Sex { get; private set; }

        public bool MarketEmail { get; private set; }

        public long?  UniversityId { get; private set; }

        public string Culture { get; private set; }

        public abstract string CommandResolveName { get; }
    }
}
