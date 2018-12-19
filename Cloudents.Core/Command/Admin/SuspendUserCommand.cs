using System;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class SuspendUserCommand : ICommand
    {
        public SuspendUserCommand(long id, bool shouldDeleteData, DateTimeOffset? lockoutEnd)
        {
            Id = id;
            ShouldDeleteData = shouldDeleteData;
            LockoutEnd = lockoutEnd;
        }
        //public string Email{ get; set; }

        public long  Id { get; }


        public bool ShouldDeleteData { get; private set; }
        public DateTimeOffset? LockoutEnd { get; set; }

    }
}
