using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class PreviewFailedCommand :ICommand
    {
        public PreviewFailedCommand(long itemId)
        {
            ItemId = itemId;
        }

        public long ItemId { get; private set; }
    }
}
