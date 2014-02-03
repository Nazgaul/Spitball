using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UnfollowBoxCommand: ICommand
    {
        public UnfollowBoxCommand(long boxId, long userId)
        {
            BoxId = boxId;
            UserId = userId;
        }
        public long BoxId { get;private set; }

        public long UserId { get;private set; }
    }
}
