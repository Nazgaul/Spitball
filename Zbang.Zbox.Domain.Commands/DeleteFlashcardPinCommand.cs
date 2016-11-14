using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteFlashcardPinCommand : ICommand
    {
        public DeleteFlashcardPinCommand(long userId, long flashCardId, int index)
        {
            UserId = userId;
            FlashCardId = flashCardId;
            Index = index;
        }

        public long UserId { get; private set; }
        public long FlashCardId { get; private set; }
        public int Index { get; private set; }
    }
}