using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class RenameNodeCommand:ICommand
    {
        public RenameNodeCommand(string newName, Guid nodeId, long universityId)
        {
            UniversityId = universityId;
            NodeId = nodeId;
            NewName = newName;
        }

        public string NewName { get; private set; }
        public Guid NodeId { get; private set; }
        public long UniversityId { get; private set; }
    }
}
