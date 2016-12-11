using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateFlashcardCommand : ICommandAsync
    {
        public UpdateFlashcardCommand(Flashcard flashcard)
        {
            Flashcard = flashcard;
        }

        public Flashcard Flashcard { get; private set; }
    }

    public class DeleteFlashcardCommand : ICommandAsync
    {
        public DeleteFlashcardCommand(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        public long Id { get; private set; }
        public long UserId { get; private set; }
    }
}