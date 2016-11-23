using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class SaveUserFlashcardCommand : ICommand
    {
        public SaveUserFlashcardCommand(long flashcardId, long userId)
        {
            FlashcardId = flashcardId;
            UserId = userId;
        }

        public long UserId { get; private set; }
        public long FlashcardId { get; private set; }
    }
}
