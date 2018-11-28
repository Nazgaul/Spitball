using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class UnSuspendUserCommand : ICommand
    {
        public UnSuspendUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
            
    }
}
