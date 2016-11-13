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

    public class PublishFlashcardCommand : ICommandAsync
    {
        public PublishFlashcardCommand(Flashcard flashcard)
        {
            Flashcard = flashcard;
        }

        public Flashcard Flashcard { get; private set; }
    }
}