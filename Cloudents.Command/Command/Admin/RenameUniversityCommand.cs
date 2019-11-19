using System;

namespace Cloudents.Command.Command.Admin
{
    public class RenameUniversityCommand : ICommand
    {
        public RenameUniversityCommand(Guid universityId, string newName)
        {
            UniversityId = universityId;
            NewName = newName;
        }
        public Guid UniversityId { get; }
        public string NewName { get; }
    }
}
