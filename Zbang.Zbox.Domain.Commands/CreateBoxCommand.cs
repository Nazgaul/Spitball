using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateBoxCommand : ICommand
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
