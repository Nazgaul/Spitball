using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
   public class UnsubscribeUsersFromEmailCommand : ICommand
    {
       public UnsubscribeUsersFromEmailCommand(IEnumerable<string> emails, EmailSend type)
       {
           Emails = emails;
           Type = type;
       }

       public IEnumerable<string> Emails { get; private set; }

       public EmailSend Type { get; private set; }
    }
}
