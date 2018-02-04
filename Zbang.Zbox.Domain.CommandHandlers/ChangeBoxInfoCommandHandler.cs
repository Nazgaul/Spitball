using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeBoxInfoCommandHandler : ICommandHandler<ChangeBoxInfoCommand>
    {
        private readonly IBoxRepository _boxRepository;
        private readonly IRepository<AcademicBox> m_AcademicBoxRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserBoxRelRepository m_UserboxRelationshipRepository;
        public ChangeBoxInfoCommandHandler(IBoxRepository boxRepository, IRepository<AcademicBox> academicBoxRepository,
            IUserRepository userRepository,
            IUserBoxRelRepository userBoxRelRepository)
        {
            _boxRepository = boxRepository;
            _userRepository = userRepository;
            m_UserboxRelationshipRepository = userBoxRelRepository;
            m_AcademicBoxRepository = academicBoxRepository;
        }

        public void Handle(ChangeBoxInfoCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var box = _boxRepository.Load(command.BoxId);
            var user = _userRepository.Load(command.UserId);
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed " + Box.NameLength);
            }
            var academicBox = box.Actual as AcademicBox;
            if (academicBox != null)
            {
                academicBox.UpdateBoxInfo(command.CourseCode, command.ProfessorName);

                m_AcademicBoxRepository.Save(academicBox);
            }

            var boxNameExists = box.Owner.GetUserOwnedBoxes().FirstOrDefault(w => w.Name == command.BoxName.Trim() && w.Id != box.Id);
            if (boxNameExists != null)
                throw new ArgumentException("box with that name already exists");

            box.ChangeBoxName(command.BoxName, user);
            _boxRepository.UpdateItemsToDirty(box.Id);
            //foreach (var item in box.Items)
            //{
            //    item.ShouldMakeDirty = () => true;
            //    _itemRepository.Save(item);
            //}
            foreach (var flashcard in box.Flashcards)
            {
                flashcard.ShouldMakeDirty = () => true;
            }
            if (command.Privacy.HasValue)
            {
                box.ChangePrivacySettings(command.Privacy.Value, user);
            }
            ChangeNotificationSettings(command.UserId, command.BoxId, command.Notification);

            _boxRepository.Save(box);
        }

        private void ChangeNotificationSettings(long userId, long boxId, NotificationSetting? notificationSettings)
        {
            if (!notificationSettings.HasValue)
            {
                return;
            }
            var userBoxRel = m_UserboxRelationshipRepository.GetUserBoxRelationship(userId, boxId);
            if (userBoxRel == null)
            {
                throw new ArgumentException("User is not connected to the box");
            }
            userBoxRel.NotificationSettings = notificationSettings.Value;
            m_UserboxRelationshipRepository.Save(userBoxRel);
        }
    }
}
