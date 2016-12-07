
using System.Collections.Generic;
using System.Threading;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateReputationCommand : ICommand
    {
        public UpdateReputationCommand(IEnumerable<long> userIds, CancellationToken token)
        {
            UserIds = userIds;
            Token = token;
        }

        public IEnumerable<long> UserIds { get; private set; }

        public CancellationToken Token { get; private set; }
    }
}
