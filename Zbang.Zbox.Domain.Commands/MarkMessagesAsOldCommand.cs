using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class MarkMessagesAsOldCommand : ICommand
    {
        public MarkMessagesAsOldCommand(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }
    }
}
