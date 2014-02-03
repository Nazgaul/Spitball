using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserFirstTimeStatusCommand : ICommand
    {
        public UpdateUserFirstTimeStatusCommand(FirstTime firstTimeStage, long userId)
        {
            FirstTimeStage = firstTimeStage;
            UserId = userId;
        }

        public FirstTime FirstTimeStage { get; private set; }
        public long UserId { get; private set; }
    }
}
