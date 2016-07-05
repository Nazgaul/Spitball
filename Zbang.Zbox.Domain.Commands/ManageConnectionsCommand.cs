using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ManageConnectionsCommand : ICommand
    {
        public ManageConnectionsCommand(IEnumerable<string> connectionIds)
        {
            ConnectionIds = connectionIds;
        }

        public IEnumerable<string> ConnectionIds { get; private set; }
    }
}
