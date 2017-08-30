using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateUniversityCommand : ICommandAsync
    {
        public CreateUniversityCommand(string name, string country, long userId)
        {
            UserId = userId;
            Name = name;
            Country = country;
        }
        public string Name { get; private set; }
        public string Country { get; private set; }

        public long Id { get; set; }

        public long UserId { get; private set; }

    }
}
