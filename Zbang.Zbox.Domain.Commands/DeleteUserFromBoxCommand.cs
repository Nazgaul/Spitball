using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class DeleteUserFromBoxCommand : ICommand
    {
        //Ctors
        public DeleteUserFromBoxCommand(long userId, long userToDeleteId, long boxId)
        {
            UserId = userId;
            UserToDeleteId = userToDeleteId;
            BoxId = boxId;

        }

        //Properties
        [DataMember]
        public long UserId { get; private set; }

        [DataMember]
        public long UserToDeleteId { get; private set; }

        [DataMember]
        public long BoxId { get; private set; }

    }
}
