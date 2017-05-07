using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeNotificationSettingsCommandHandler : ICommandHandler<ChangeNotificationSettingsCommand>
    {
        private readonly IUserBoxRelRepository m_UserBoxRelationshipRepository;


        public ChangeNotificationSettingsCommandHandler(IUserBoxRelRepository userBoxRelationshipRepository)
        {

            m_UserBoxRelationshipRepository = userBoxRelationshipRepository;
        }

        public void Handle(ChangeNotificationSettingsCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var userBoxRel = m_UserBoxRelationshipRepository.GetUserBoxRelationship(command.UserId, command.BoxId);
            userBoxRel.NotificationSettings = command.NewNotificationSettings;
        }
    }
}
