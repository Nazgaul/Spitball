using Cloudents.Core.Interfaces;
using System.Collections.Generic;

namespace Cloudents.Core.Command.Admin
{
    public class UpdateUserBalanceCommand : ICommand
    {
        public UpdateUserBalanceCommand(IEnumerable<long> usersIds)
        {
            UsersIds = usersIds;
        }
        public IEnumerable<long> UsersIds { get; set; }
    }
}
