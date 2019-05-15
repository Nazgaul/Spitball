using System;

namespace Cloudents.Core.DTOs
{
    public class StudyRoomDto
    {
       
        public string OnlineDocument { get; set; }
        public string ConversationId { get; set; }

        public long TutorId { get; set; }

        public bool AllowReview { get; set; }
        public bool NeedPayment => false;
    }


    public class UserStudyRoomDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public long UserId { get; set; }
        public bool Online { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public Guid ConversationId { get; set; }

        
    }
}