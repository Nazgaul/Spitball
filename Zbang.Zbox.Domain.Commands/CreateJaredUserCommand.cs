using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class CreateJaredUserCommand : ICommand
    {
        public CreateJaredUserCommand(Guid userIdToken)
        {
            UserIdToken = userIdToken;
        }


        public Guid UserIdToken { get; private set; }
    }

    public class CreateJaredUserCommandResult : ICommandResult
    {
        public CreateJaredUserCommandResult(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
