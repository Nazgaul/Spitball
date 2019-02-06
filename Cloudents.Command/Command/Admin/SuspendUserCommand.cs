using System;

namespace Cloudents.Command.Command.Admin
{
    public class SuspendUserCommand : ICommand
    {
        public SuspendUserCommand(long id, DateTimeOffset lockoutEnd)
        {
            Id = id;
            LockoutEnd = lockoutEnd;
        }
        //public string Email{ get; set; }

        public long  Id { get; }
        public DateTimeOffset LockoutEnd { get; set; }

    }
}
