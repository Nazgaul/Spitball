﻿namespace Cloudents.Command.Command
{
    public class ReferringUserCommand : ICommand
    {
        public ReferringUserCommand(long invitingUserId, long registeredUserId)
        {
            InvitingUserId = invitingUserId;
            RegisteredUserId = registeredUserId;
        }

        public long InvitingUserId { get; }
        public long RegisteredUserId { get; }
    }
}