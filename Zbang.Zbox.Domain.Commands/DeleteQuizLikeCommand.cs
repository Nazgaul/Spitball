using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteQuizLikeCommand : ICommandAsync
    {
        public DeleteQuizLikeCommand(long userId, Guid id)
        {
            UserId = userId;
            Id = id;
        }

        public long UserId { get; private set; }
        public Guid Id { get; private set; }

    }
}