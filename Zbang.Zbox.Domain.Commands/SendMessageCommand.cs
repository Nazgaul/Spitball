using System.Collections.Generic;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class SendMessageCommand : ICommand
    {
        public SendMessageCommand(long sender, IList<string> inviteEmailList,
            string personalNote//, string url
            )
        {
          //  Url = url;
            Sender = sender;
            Recepients = inviteEmailList;
            PersonalNote = personalNote;
        }

        //public string Url { get; private set; }
        public long Sender { get; private set; }
        public IList<string> Recepients { get; private set; }
        public string PersonalNote { get; private set; }
    }
}
