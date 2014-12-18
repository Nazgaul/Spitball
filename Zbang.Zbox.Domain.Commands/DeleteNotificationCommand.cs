using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteNotificationCommand : ICommand
    {
        public DeleteNotificationCommand(Guid messageId)
        {
            MessageId = messageId;
        }

        public Guid MessageId { get; private set; }

    }
}
