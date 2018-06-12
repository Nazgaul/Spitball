namespace Cloudents.Core.DTOs
{
    public class QuestionDto
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public decimal Price { get; set; }
        public string Text { get; set; }

        public int Files { get; set; }
        public int Answers { get; set; }
        public string UserName { get; set; }
    }
}