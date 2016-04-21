namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteUpdatesItemCommand : DeleteUpdatesCommand
    {
        public DeleteUpdatesItemCommand(long userId, long boxId)
            :base(userId,boxId)
        {
            
        }
    }
}