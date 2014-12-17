using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddAnnotationCommand : ICommandAsync
    {
        public AddAnnotationCommand(string comment,  long itemId,  long userId, long boxId)
        {
            BoxId = boxId;
            Comment = comment;
            ItemId = itemId;
            UserId = userId;
        }
        public string Comment { get; private set; }

      
        public long ItemId { get; private set; }

        public long UserId { get; private set; }

        //out parameter
        public long AnnotationId { get; set; }

        public long BoxId { get; private set; }
    }
}
