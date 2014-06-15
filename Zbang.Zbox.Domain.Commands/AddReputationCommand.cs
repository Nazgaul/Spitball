using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddReputationCommand : ICommand
    {
        public AddReputationCommand(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }
    }
}
