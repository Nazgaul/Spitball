using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateUniversityCommand : ICommand
    {
        public CreateUniversityCommand(string name, string country, string smallImage, string largeImage, long userId)
        {
            //"https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/Lib1.jpg",
            //"https://zboxstorage.blob.core.windows.net/zboxprofilepic/S100X100/Lib1.jpg"
            UserId = userId;
            LargeImage = largeImage;
            SmallImage = smallImage;
            Name = name;
            Country = country;
        }
        public string Name { get; private set; }
        public string Country { get; private set; }

        public long Id { get; set; }

        public string SmallImage { get; private set; }
        public string LargeImage { get; private set; }
        public long UserId { get; private set; }

    }
}
