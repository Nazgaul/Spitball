using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Commands;
namespace Zbang.Zbox.Domain.Commands
{
    [DataContract]
    public class SubscribeToSharedBoxCommand : ICommand
    {
        public SubscribeToSharedBoxCommand(long Id, long boxId)
        {
            this.Id = Id;
            BoxId = boxId;
        }

        [DataMember]
        public long Id { get; private set; }

        [DataMember]
        public long BoxId { get; private set; }

        
    }
}