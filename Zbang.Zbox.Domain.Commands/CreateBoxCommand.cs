using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateBoxCommand : ICommandAsync
    {

        public CreateBoxCommand(long userId, string boxName)
        {
            UserId = userId;
            BoxName = boxName;
           
        }

        public long UserId { get; private set; }
        public string BoxName { get; private set; }

    }
}
