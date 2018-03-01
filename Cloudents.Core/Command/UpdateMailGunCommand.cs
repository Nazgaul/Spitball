using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class UpdateMailGunCommand : ICommand
    {
        public UpdateMailGunCommand(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}