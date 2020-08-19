namespace Cloudents.Web.Models
{
    public class FinishChatUpload : UploadRequestFinish
    {
        public long OtherUser { get; set; }

        public long? TutorId { get; set; }

        public string ConversationId { get; set; }
    }
}