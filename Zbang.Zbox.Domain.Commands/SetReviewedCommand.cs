using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class SetReviewedCommand:ICommand
    {
        public SetReviewedCommand(long item)
        {
            ItemId = item;
        }
        public long ItemId { get; private set; }
    }
    
   
}
