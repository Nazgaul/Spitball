using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class StudyRoomDto
    {
        public Guid SessionId { get; set; }
        public string TutorName { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public string Duration { get; set; }
        public long TutorId { get; set; }
        public long UserId { get; set; }
    }
}
