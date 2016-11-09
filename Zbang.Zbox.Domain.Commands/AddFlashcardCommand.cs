using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFlashcardCommand : ICommandAsync
    {
        public AddFlashcardCommand(Flashcard flashcard)
        {
            Flashcard = flashcard;
        }

        public Flashcard Flashcard { get; private set; }
    }
}
