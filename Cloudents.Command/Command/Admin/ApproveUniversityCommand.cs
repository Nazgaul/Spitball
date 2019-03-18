using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveUniversityCommand: ICommand
    {
        public ApproveUniversityCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }
    }
}
