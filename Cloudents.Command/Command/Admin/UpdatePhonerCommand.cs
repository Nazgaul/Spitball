using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class UpdatePhonerCommand : ICommand
    {
        public UpdatePhonerCommand(long userId, string newPhone)
        {
            UserId = userId;
            NewPhone = newPhone;
        }
        public long UserId{ get; }
        public string NewPhone { get; }
    }
}
