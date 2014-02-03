using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeBoxPrivacySettingsCommandHandler : ICommandHandler<ChangeBoxPrivacySettingsCommand, ChangeBoxPrivacySettingsCommandResult>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;

        public ChangeBoxPrivacySettingsCommandHandler(IRepository<Box> boxRepository, IUserRepository userRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
        }

        public ChangeBoxPrivacySettingsCommandResult Execute(ChangeBoxPrivacySettingsCommand command)
        {
            Box box = m_BoxRepository.Get(command.BoxId);
            
            if (command.NewSettings == box.PrivacySettings.PrivacySetting)
            {
                return new ChangeBoxPrivacySettingsCommandResult(box, false);
            }
            var userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, box.Id); //user.GetUserType(box.Id);
            if (userType != UserRelationshipType.Owner)
                throw new UnauthorizedAccessException("Only the owner of the box may change its privacy settings");



            box.PrivacySettings.PrivacySetting = command.NewSettings;

            m_BoxRepository.Save(box);

            return new ChangeBoxPrivacySettingsCommandResult(box, true);

        }
    }
}
