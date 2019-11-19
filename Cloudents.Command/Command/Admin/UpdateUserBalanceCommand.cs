using System.Collections.Generic;

namespace Cloudents.Command.Command.Admin
{
    public class UpdateUserBalanceCommand : ICommand
    {
        public UpdateUserBalanceCommand(IEnumerable<long> usersIds)
        {
            UsersIds = usersIds;
        }
        public IEnumerable<long> UsersIds { get; }
    }
}
