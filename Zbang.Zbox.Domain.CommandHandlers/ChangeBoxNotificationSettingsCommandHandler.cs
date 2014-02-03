using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeBoxNotificationSettingsCommandHandler : ICommandHandler<ChangeBoxNotificationSettingsCommand>
    {
        private readonly IUserBoxRelRepository m_UserBoxRepository;
        public ChangeBoxNotificationSettingsCommandHandler(IUserBoxRelRepository userBoxRepository)
        {
            m_UserBoxRepository = userBoxRepository;
        }
        public void Handle(ChangeBoxNotificationSettingsCommand message)
        {
            var userBox = m_UserBoxRepository.GetUserBoxRelationship(message.UserId,message.BoxId);
            userBox.NotificationSettings = message.NotificationSetting;

            m_UserBoxRepository.Save(userBox);

        }
    }
}
