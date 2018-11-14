using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class DeleteUserCommand : ICommand
    {
        public DeleteUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
