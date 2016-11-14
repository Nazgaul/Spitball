using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFlashcardPinCommand : ICommand
    {
        public AddFlashcardPinCommand(long userId, long flashCardId, int index)
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
