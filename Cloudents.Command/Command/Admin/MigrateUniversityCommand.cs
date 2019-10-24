using System;

namespace Cloudents.Command.Command.Admin
{
    public class MigrateUniversityCommand : ICommand
    {
        public MigrateUniversityCommand(Guid universityToKeep, Guid universityToRemove)
        {
            UniversityToKeep = universityToKeep;
            UniversityToRemove = universityToRemove;
        }
        public Guid UniversityToKeep { get; set; }
        public Guid UniversityToRemove { get; set; }
    }
}
