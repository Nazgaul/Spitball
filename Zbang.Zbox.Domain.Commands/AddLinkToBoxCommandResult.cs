using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddLinkToBoxCommandResult : AddItemToBoxCommandResult
    {
        public AddLinkToBoxCommandResult(Item link)
        {
            Link = link;
        }

        public Item Link { get; private set; }
    }
}
