using System;

namespace Cloudents.Command.Command.Admin
{
    public class SuspendUserCommand : ICommand
    {
        public SuspendUserCommand(long id, DateTimeOffset lockoutEnd, string reason)
        {
            Id = id;
            LockoutEnd = lockoutEnd;
            Reason = reason;
        }
        //public string Email{ get; set; }

        public long Id { get; }
        public DateTimeOffset LockoutEnd { get; }
        public string Reason { get; }

    }
}
