﻿using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class ChangeBoxInfoCommandHandler : ICommandHandler<ChangeBoxInfoCommand>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<AcademicBox> m_AcademicBoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IUserBoxRelRepository m_UserboxRelationshipRepository;
        public ChangeBoxInfoCommandHandler(IRepository<Box> boxRepository, IRepository<AcademicBox> academicBoxRepository,
            IUserRepository userRepository,
            IUserBoxRelRepository userBoxRelRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_UserboxRelationshipRepository = userBoxRelRepository;
            m_AcademicBoxRepository = academicBoxRepository;
        }
        public void Handle(ChangeBoxInfoCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            // 

            Box box = m_BoxRepository.Get(command.BoxId); // need to get not to get proxy
            User user = m_UserRepository.Load(command.UserId);

            var academicBox = box as AcademicBox;
            if (academicBox != null)
            {
                academicBox.UpdateBoxInfo(command.CourseCode, command.ProfessorName);
                m_AcademicBoxRepository.Save(academicBox);
            }
            
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed " + Box.NameLength);
            }
            var boxNameExists = box.Owner.GetUserOwnedBoxes().FirstOrDefault(w => w.Name == command.BoxName.Trim() && w.Id != box.Id);
            if (boxNameExists != null)
                throw new ArgumentException("box with that name already exists");
            
            box.ChangeBoxName(command.BoxName,user);
            if (command.Privacy.HasValue)
            {
                box.ChangePrivacySettings(command.Privacy.Value, user);
            }
            ChangeNotificationSettings(command.UserId, command.BoxId, command.Notification);
          
            m_BoxRepository.Save(box);
        }


        private void ChangeNotificationSettings(long userId, long boxId, NotificationSettings? notificationSettings)
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
