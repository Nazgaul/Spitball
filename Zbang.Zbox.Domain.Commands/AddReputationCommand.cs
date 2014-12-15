using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddReputationCommand : ICommand
    {
        public AddReputationCommand(long userId, ReputationAction reputationAction)
        {
            ReputationAction = reputationAction;
            UserId = userId;
        }

        public long UserId { get; private set; }

        public ReputationAction ReputationAction { get; private set; }
    }
}
