using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
   public class ChangeBoxLibraryCommand : ICommand
    {
       public ChangeBoxLibraryCommand(IEnumerable<long> boxIds, Guid libraryId)
       {
           BoxIds = boxIds;
           LibraryId = libraryId;
       }

       public IEnumerable<long> BoxIds { get; private set; }

       public Guid LibraryId { get; private set; }
    }
}
