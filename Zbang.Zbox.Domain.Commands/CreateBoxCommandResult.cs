
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateBoxCommandResult: ICommandResult
    {
        public CreateBoxCommandResult(Box box, string userName)
        {
            NewBox = box;
            UserName = userName;
        }

        public Box NewBox { get; private set; }

        public string UserName { get; private set; }
    }
}
