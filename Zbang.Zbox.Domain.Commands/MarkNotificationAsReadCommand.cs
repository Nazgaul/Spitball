using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class MarkNotificationAsReadCommand : ICommand
    {
        public MarkNotificationAsReadCommand(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }
    }
}
