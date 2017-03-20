using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public abstract class SetReviewedCommand:ICommand
    {
        public SetReviewedCommand(long item)
        {
            ItemId = item;
        }
        public long ItemId { get; private set; }
    }
    public class SetReviewedDocumentCommand : SetReviewedCommand
    {
        public SetReviewedDocumentCommand(long item) : base(item)
        {
        }
    }
    public class SetReviewedQuizCommand : SetReviewedCommand
    {
        public SetReviewedQuizCommand(long item) : base(item)
        {
        }
    }
    public class SetReviewedFlashcardCommand : SetReviewedCommand
    {
        public SetReviewedFlashcardCommand(long item) : base(item)
        {
        }
    }
}
