using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateUniversityCommand : ICommand
    {
        public CreateUniversityCommand(string name, string email, string country)
        {
            Name = name;
            Email = email;
            Country = country;
        }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Country { get; private set; }
    }
}
