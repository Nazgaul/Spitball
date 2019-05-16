using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class ApproveTutorCommand: ICommand
    {
        public ApproveTutorCommand(long id)
        {
            Id = id;
        }
        public long Id { get; set; }
    }
}
