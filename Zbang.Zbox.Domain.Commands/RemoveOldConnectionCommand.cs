using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class RemoveOldConnectionCommand : ICommand
    {
        public IList<long> UserIds { get; set; }
    }
}
