namespace Cloudents.Core.DTOs.Email
{
    public class RequestTutorAdminEmailDto
    {
        public string CourseName { get; set; }

        public string TutorName { get; set; }
        public string UserPhone { get; set; }
        public long UserId { get; set; }
    }
}
