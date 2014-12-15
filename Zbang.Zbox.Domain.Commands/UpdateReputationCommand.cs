
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateReputationCommand : ICommand
    {
        public UpdateReputationCommand(long userId)
        {
            UserId = userId;
        }

        public long UserId { get;private set; }
    }
}
