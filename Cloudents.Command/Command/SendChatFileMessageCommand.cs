using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class SendChatFileMessageCommand : ICommand
    {
        public SendChatFileMessageCommand(string blob, long userSendingId,
           long? tutorId,
            string? identifier = null)
        {
            Blob = blob;
            UserSendingId = userSendingId;
            TutorId = tutorId;
            Blob = blob;
            Identifier = identifier;
        }


        public long UserSendingId { get; }

        public string Blob { get; }
        public string? Identifier { get; }
        public long? TutorId { get; }
        //public IEnumerable<long> ToUsersId { get; }
    }
}