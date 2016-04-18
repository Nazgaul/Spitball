using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
   public class UnsubscribeUsersFromEmailCommand : ICommand
    {
       public UnsubscribeUsersFromEmailCommand(IEnumerable<string> emails)
       {
           Emails = emails;
       }

       public IEnumerable<string> Emails { get; private set; }
    }
}
