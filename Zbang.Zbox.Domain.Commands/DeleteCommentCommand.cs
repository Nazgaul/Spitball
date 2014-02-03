using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class DeleteCommentCommand : ICommand
    {

        //Ctors
        public DeleteCommentCommand(long commentId, long userId, long boxId)
        {
            CommentId = commentId;
            UserId = userId;
            BoxId = boxId;
        }

        //Properties
        [DataMember]
        public long CommentId { get; private set; }

        //Properties
        [DataMember]
        public long UserId { get; private set; }

        [DataMember]
        public long BoxId { get; set; }

       
    }
}
