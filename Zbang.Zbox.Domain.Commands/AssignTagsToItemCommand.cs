using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class AssignTagsToItemCommand : ICommand
    {
        protected AssignTagsToItemCommand(long itemId, IEnumerable<string> tags, TagType type)
        {
            ItemId = itemId;
            Tags = tags;
            Type = type;
        }

        public long ItemId { get; private set; }
        public IEnumerable<string> Tags { get; private set; }

        public TagType Type { get; private set; }
    }

    public class AssignTagsToDocumentCommand : AssignTagsToItemCommand
    {
        public AssignTagsToDocumentCommand(long itemId, IEnumerable<string> tags, TagType type) :base(itemId,tags, type)
        {
            
        }
    }
    public class AssignTagsToFlashcardCommand : AssignTagsToItemCommand
    {
        public AssignTagsToFlashcardCommand(long itemId, IEnumerable<string> tags, TagType type) : base(itemId, tags, type)
        {

        }
    }
    public class AssignTagsToQuizCommand : AssignTagsToItemCommand
    {
        public AssignTagsToQuizCommand(long itemId, IEnumerable<string> tags, TagType type) : base(itemId, tags, type)
        {

        }
    }
}
