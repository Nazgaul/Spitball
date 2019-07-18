using System;

namespace Cloudents.Core.DTOs
{
    public class StudyRoomDto
    {
       
        public string OnlineDocument { get; set; }
        public string ConversationId { get; set; }
        public long TutorId { get; set; }
        public string TutorImage { get; set; }
        public string TutorName { get; set; }
        public long StudentId { get; set; }
        public string StudentImage { get; set; }
        public string StudentName { get; set; }
        public bool AllowReview => true;

        public bool CardEnter { get; set; }
        public bool NeedPayment { get; set; }
    };
       


    public class UserStudyRoomDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public long UserId { get; set; }
        public bool Online { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }

        public string ConversationId { get; set; }

        
    }
}