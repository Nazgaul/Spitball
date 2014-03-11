using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteUpdatesCommand :ICommand
    {
        public DeleteUpdatesCommand(long userId, long boxId)
        {
            UserId = userId;
            BoxId = boxId;
        }
        public long UserId { get; private set; }
        public long BoxId { get;private set; }
    }
}
