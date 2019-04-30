using System;

namespace Cloudents.Command.Command.Admin
{
    public class RenameUniversityCommand: ICommand
    {
        public RenameUniversityCommand(Guid universityId, string newName)
        {
            UniversityId = universityId;
            NewName = newName;
        }
        public Guid UniversityId { get; set; }
        public string NewName { get; set; }
    }
}
