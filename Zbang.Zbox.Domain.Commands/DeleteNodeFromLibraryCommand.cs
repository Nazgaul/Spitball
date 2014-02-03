using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteNodeFromLibraryCommand :ICommand
    {
        public DeleteNodeFromLibraryCommand(Guid nodeId, long universityId)
        {
            UniversityId = universityId;
            NodeId = nodeId;
        }

        public Guid NodeId { get; private set; }
        public long UniversityId { get; private set; }
    }
}
