using System;
using System.Collections.Generic;
using System.Text;

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
