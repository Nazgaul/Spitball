using System;

namespace Cloudents.Core.DTOs
{
    public abstract class UserContentDto
    {
        public virtual string Type { get; set; }
        //public ItemState State { get; set; }
        public DateTime Date { get; set; }
        public string Course { get; set; }
    }

    public class UserDocumentsDto : UserContentDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Downloads{ get; set; }
        public int Purchased { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
    }

    public class UserQuestionsDto : UserContentDto
    {
        public long Id { get; set; }
        public override string Type => "Question";
        public string Text { get; set; }
        public string AnswerText { get; set; }
    }

    public class UserAnswersDto : UserContentDto
    {
        public long QuestionId { get; set; }
        public override string Type => "Answer";
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
    }
}
