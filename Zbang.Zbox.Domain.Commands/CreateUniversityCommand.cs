using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateUniversityCommand : ICommand
    {
        public CreateUniversityCommand(long id, string name, string country, string smallImage, string largeImage, long userId)
        {
            UserId = userId;
            LargeImage = largeImage;
            SmallImage = smallImage;
            Id = id;
            Name = name;
            Country = country;
        }
        public string Name { get; private set; }
        public string Country { get; private set; }

        public long Id { get; private set; }

        public string SmallImage { get; private set; }
        public string LargeImage { get; private set; }
        public long UserId { get; private set; }

    }
}
