using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
   public  class UpdateItemWithNoSizeCommand : ICommandAsync
    {
       public UpdateItemWithNoSizeCommand(IEnumerable<long> itemIds)
       {
           ItemIds = itemIds;
       }

       public IEnumerable<long> ItemIds { get; private set; }
    }
}
