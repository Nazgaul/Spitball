using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities
{
    public class ViewConversation
    {
        protected ViewConversation()
        {
            
        }
        public virtual string Id { get; set; }
        public virtual DateTime LastMessage { get; set; }
        public virtual string UserName { get; set; }
        public virtual string UserPhoneNumber { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual long UserId { get; set; }
        public virtual string TutorName { get; set; }
        public virtual string Country { get; set; }
        public virtual string TutorPhoneNumber { get; set; }
        public virtual string TutorEmail { get; set; }
        public virtual long TutorId { get; set; }
        public virtual ChatRoomStatus Status { get; set; }
        public virtual string AssignTo { get; set; }
        public virtual string RequestFor { get; set; }
        public virtual int ConversationStatus { get; set; }
        public virtual bool StudyRoomExists { get; set; }
        public virtual int HoursFromLastMessage { get; set; }
    }
}