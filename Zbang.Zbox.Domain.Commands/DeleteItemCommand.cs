
using System.Runtime.Serialization;

using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class DeleteItemCommand : ICommand
    {

        //Ctors
        public DeleteItemCommand(long itemId, long userId, long boxId)
        {
            ItemId = itemId;
            UserId = userId;
            BoxId = boxId;
        }

        //Properties
        [DataMember]
        public long ItemId
        {
            get;
            private set;

        }
        [DataMember]
        public long BoxId { get; private set; }

        [DataMember]
        public long UserId { get; private set; }

        
    }
}
