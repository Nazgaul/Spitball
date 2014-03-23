using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class MarkMessagesAsReadCommand : ICommand
    {
        public MarkMessagesAsReadCommand(long userId, Guid messageId)
        {
            UserId = userId;
            MessageId = messageId;
        }
        public long UserId { get; private set; }
        public Guid MessageId { get; private set; }
    }
}
