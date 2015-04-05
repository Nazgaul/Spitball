﻿
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeBoxInfoCommand : ICommand
    {
        public ChangeBoxInfoCommand(long boxId, 
            long userId, 
            string boxName, 
            string professorName,
            string courseCode,
            BoxPrivacySettings? privacy,
            NotificationSettings? notification)
        {
            BoxId = boxId;
            UserId = userId;
            BoxName = boxName;
            ProfessorName = professorName;
            CourseCode = courseCode;

            Privacy = privacy;
            Notification = notification;
        }

        public long BoxId { get; private set; }

        public long UserId { get; private set; }

        public string BoxName { get; private set; }

        public string ProfessorName { get; private set; }
        public string CourseCode { get; private set; }

        public BoxPrivacySettings? Privacy { get; private set; }
        public NotificationSettings? Notification { get; private set; }

    }
}
