using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserCommandResult : ICommandResult
    {
       

        public UpdateUserCommandResult()
        {

        }

        public string Error { get; set; }
    }
}
