using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteFlashcardLikeCommand : ICommandAsync
    {
        public DeleteFlashcardLikeCommand(long userId, Guid id/*, long flashCardId*/)
        {
            UserId = userId;
            Id = id;
            // FlashCardId = flashCardId;
        }

        public long UserId { get; private set; }
        public Guid Id { get; private set; }
        // public long FlashCardId { get; private set; }

    }
}