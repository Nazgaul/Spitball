using System.Collections.Generic;


namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateDirtyToRegularCommand
    {
        public UpdateDirtyToRegularCommand(IEnumerable<long> ids)
        {
            Ids = ids;
        }

        public IEnumerable<long> Ids { get; private set; }
    }
}
