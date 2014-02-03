using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class ChangeNotificationSettingsCommandHandler : ICommandHandler<ChangeNotificationSettingsCommand>
    {

        readonly IUserBoxRelRepository m_UserboxRelationshipRepository;


        public ChangeNotificationSettingsCommandHandler(IUserBoxRelRepository userboxRelationshipRepository)
        {

            m_UserboxRelationshipRepository = userboxRelationshipRepository;
        }

        public void Handle(ChangeNotificationSettingsCommand command)
        {
            var userBoxRel = m_UserboxRelationshipRepository.GetUserBoxRelationship(command.UserId, command.BoxId);
            userBoxRel.NotificationSettings = command.NewNotificationSettings;
        }
    }
}
