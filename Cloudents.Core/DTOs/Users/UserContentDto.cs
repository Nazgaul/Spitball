using System;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Users
{
    public class UserCoursesDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime StartOn { get; set; }

        public int Lessons { get; set; }

        public int Documents { get; set; }

        public int Users { get; set; }

        public Money Price { get; set; }
        public bool IsPublish { get; set; }
    }

    //public class UserDocumentsDto : UserContentDto
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public decimal Price { get; set; }
    //    public int Views { get; set; }
    //    public int Likes => 0;
    //    public int Downloads{ get; set; }
    //    public int Purchased { get; set; }
    //    public string Preview { get; set; }
    //    public string Url { get; set; }
    //}

    //public class UserQuestionsDto : UserContentDto
    //{
    //    public long Id { get; set; }
    //    public override string Type => "Question";
    //    public string Text { get; set; }
    //    public string AnswerText { get; set; }
    //}

    //public class UserAnswersDto : UserContentDto
    //{
    //    public long QuestionId { get; set; }
    //    public override string Type => "Answer";
    //    public string QuestionText { get; set; }
    //    public string AnswerText { get; set; }
    //}
}
