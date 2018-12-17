namespace Cloudents.Web.Models
{
    public class QuestionSubjectResponse
    {
        public QuestionSubjectResponse(string id, string subject)
        {
            Id = id;
            Subject = subject;
        }

        public string Id { get; set; }
        public string Subject { get; set; }
    }
}
