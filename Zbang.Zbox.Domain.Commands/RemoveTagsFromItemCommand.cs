using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class RemoveTagsFromItemCommand: ICommand
    {
        protected RemoveTagsFromItemCommand(long itemId, IEnumerable<string> tags)
        {
            ItemId = itemId;
            Tags = tags;
        }

        public long ItemId { get; private set; }
        public IEnumerable<string> Tags { get; private set; }
    }

    public class RemoveTagsFromDocumentCommand : RemoveTagsFromItemCommand
    {
        public RemoveTagsFromDocumentCommand(long itemId, IEnumerable<string> tags) : base(itemId, tags)
        {

        }
    }
    public class RemoveTagsFromFlashcardCommand : RemoveTagsFromItemCommand
    {
        public RemoveTagsFromFlashcardCommand(long itemId, IEnumerable<string> tags) : base(itemId, tags)
        {

        }
    }
    public class RemoveTagsFromQuizCommand : RemoveTagsFromItemCommand
    {
        public RemoveTagsFromQuizCommand(long itemId, IEnumerable<string> tags) : base(itemId, tags)
        {

        }
    }
}
