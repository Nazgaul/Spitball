using System.Collections.Generic;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
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
