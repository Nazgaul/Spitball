using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

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
}
