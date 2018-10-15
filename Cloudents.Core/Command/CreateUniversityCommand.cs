using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateUniversityCommand : ICommand
    {
        public CreateUniversityCommand(string name, long userId)
        {
            Name = name;
            UserId = userId;
        }

        public string Name { get; set; }
        public long UserId { get; set; }
    }
}