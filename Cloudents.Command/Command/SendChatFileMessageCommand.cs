using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class SendChatFileMessageCommand : ICommand
    {
        public SendChatFileMessageCommand(string blob, long userSendingId, IEnumerable<long> usersId
        )
        {
            Blob = blob;
            UserSendingId = userSendingId;
            ToUsersId = usersId;
            Blob = blob;
        }


        public long UserSendingId { get; }

        public string Blob { get; }

        public IEnumerable<long> ToUsersId { get; }
    }
}