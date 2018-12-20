using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
{
    public class ReferringUserCommand : ICommand
    {
        public ReferringUserCommand(long invitingUserId, long registeredUserId)
        {
            InvitingUserId = invitingUserId;
            RegisteredUserId = registeredUserId;
        }

        public long InvitingUserId { get; private set; }
        public long RegisteredUserId { get; private set; }
    }
}