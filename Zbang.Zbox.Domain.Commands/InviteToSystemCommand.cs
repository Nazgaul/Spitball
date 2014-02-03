using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
   public  class InviteToSystemCommand: ICommand
    {
       public InviteToSystemCommand(long senderId, IList<string> recepients)
        {
            SenderId = senderId;
            Recepients = recepients;
        }

        public long SenderId { get; private set; }
        public IList<string> Recepients { get; private set; }
    }
}
