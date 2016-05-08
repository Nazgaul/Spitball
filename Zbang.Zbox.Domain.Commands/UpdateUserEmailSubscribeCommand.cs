using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUserEmailSubscribeCommand : ICommand
    {
        public UpdateUserEmailSubscribeCommand(long userId, EmailSend sendSettings)
        {
            UserId = userId;
            SendSettings = sendSettings;
        }

        public long UserId { get;private set; }
        public EmailSend SendSettings { get; private set; }
    }
}
