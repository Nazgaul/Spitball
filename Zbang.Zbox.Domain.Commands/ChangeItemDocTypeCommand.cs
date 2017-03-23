using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeItemDocTypeCommand : ICommand
    {
        public ChangeItemDocTypeCommand(long item,ItemType docType)
        {
            ItemId = item;
            DocType = docType;
        }
        public ItemType DocType { get; set; }
        public long ItemId { get; private set; }
    }
}
