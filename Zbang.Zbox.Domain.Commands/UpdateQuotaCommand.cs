using System.Collections.Generic;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateQuotaCommand : ICommand
    {

        public UpdateQuotaCommand(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        public IEnumerable<long> UserIds { get; private set; }
    }
}
