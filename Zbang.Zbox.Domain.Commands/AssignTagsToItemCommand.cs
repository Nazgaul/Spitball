using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Zbox.Domain.Commands
{
   public  class AssignTagsToItemCommand : ICommand
    {
       public AssignTagsToItemCommand(long itemId, IEnumerable<string> tags)
       {
           ItemId = itemId;
           Tags = tags;
       }

       public long ItemId { get; private set; }
       public IEnumerable<string> Tags { get; private set; }


      
       
    }

    public class AddLanguageToDocumentCommand : ICommand
    {
        public AddLanguageToDocumentCommand(long itemId, Language language)
        {
            ItemId = itemId;
            Language = language;
        }

        public long ItemId { get; private set; }
        public Language Language { get; private set; }
    }
}
