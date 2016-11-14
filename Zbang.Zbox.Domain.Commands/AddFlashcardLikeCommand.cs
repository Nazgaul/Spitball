using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFlashcardLikeCommand : ICommand
    {
        public AddFlashcardLikeCommand(long userId, long flashcardId)
        {
            UserId = userId;
            FlashcardId = flashcardId;
        }

        public long UserId { get; private set; }
        public long FlashcardId { get; private set; }


    }
}
