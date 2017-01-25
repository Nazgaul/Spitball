using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddLanguageToDocumentCommand : AddLanguageToItemCommand
    {
        public AddLanguageToDocumentCommand(long itemId, Language language)
            :base (itemId,language)
        {
        }
    }
    public class AddLanguageToFlashcardCommand : AddLanguageToItemCommand
    {
        public AddLanguageToFlashcardCommand(long itemId, Language language)
            : base(itemId, language)
        {
        }
    }
    public class AddLanguageToQuizCommand : AddLanguageToItemCommand
    {
        public AddLanguageToQuizCommand(long itemId, Language language)
            : base(itemId, language)
        {
        }
    }

    public abstract class AddLanguageToItemCommand : ICommand
    {
        protected AddLanguageToItemCommand(long itemId, Language language)
        {
            ItemId = itemId;
            Language = language;
        }

        public long ItemId { get; private set; }
        public Language Language { get; private set; }
    }
}