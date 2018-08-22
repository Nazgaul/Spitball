using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class DeleteUserCommand : ICommand
    {
        public string Email{ get; set; }

    }
}
