namespace Cloudents.Admin2.Models
{
    public class QuestionSubjectResponse
    {
        public QuestionSubjectResponse(int id, string subject)
        {
            Id = id;
            Subject = subject;
        }

        public int Id { get; set; }
        public string Subject { get; set; }
    }
}
