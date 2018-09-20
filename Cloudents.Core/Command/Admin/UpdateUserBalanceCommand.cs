using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class UpdateUserBalanceCommand : ICommand
    {
        public UpdateUserBalanceCommand(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        public IEnumerable<long> UserIds { get; }
    }
}
