using System;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Commands;

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

    public class AddLanguageToFeedCommand : AddLanguageToItemCommand
    {
        public AddLanguageToFeedCommand(Guid itemId, Language language)
            : base(itemId, language)
        {
        }
    }

    public abstract class AddLanguageToItemCommand : ICommand
    {
        protected AddLanguageToItemCommand(object itemId, Language language)
        {
            ItemId = itemId;
            Language = language;
        }

        public object ItemId { get; private set; }
        public Language Language { get; private set; }
    }
}