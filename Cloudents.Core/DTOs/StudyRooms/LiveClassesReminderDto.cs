using System;

namespace Cloudents.Core.DTOs.StudyRooms
{
    public class LiveClassesReminderDto
    {
        public string StudyRoomTitle { get; set; }
        public string? StudyRoomDescription { get; set; }
        public Guid StudyRoomId { get; set; }
        public DateTime BroadCastTime { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentEmail { get; set; }
        public string TeacherName { get; set; }

        public string StudyRoomLink { get; set; }

        public long UserId { get; set; }
    }
}