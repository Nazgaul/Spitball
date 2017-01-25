using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class AssignTagsToItemCommand : ICommand
    {
        protected AssignTagsToItemCommand(long itemId, IEnumerable<string> tags)
        {
            ItemId = itemId;
            Tags = tags;
        }

        public long ItemId { get; private set; }
        public IEnumerable<string> Tags { get; private set; }
    }

    public class AssignTagsToDocumentCommand : AssignTagsToItemCommand
    {
        public AssignTagsToDocumentCommand(long itemId, IEnumerable<string> tags) :base(itemId,tags)
        {
            
        }
    }
    public class AssignTagsToFlashcardCommand : AssignTagsToItemCommand
    {
        public AssignTagsToFlashcardCommand(long itemId, IEnumerable<string> tags) : base(itemId, tags)
        {

        }
    }
    public class AssignTagsToQuizCommand : AssignTagsToItemCommand
    {
        public AssignTagsToQuizCommand(long itemId, IEnumerable<string> tags) : base(itemId, tags)
        {

        }
    }
}
