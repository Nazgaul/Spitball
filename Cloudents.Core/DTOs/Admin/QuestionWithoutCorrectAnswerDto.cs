using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class QuestionWithoutCorrectAnswerDto
    {


        public long QuestionId { get; set; }
       
        public string  QuestionText { get; set; }

        public string Url { get; set; }

        public bool IsFictive { get; set; }

        public AnswerOfQuestionWithoutCorrectAnswer Answer { get; set; }
    }

    public class AnswerOfQuestionWithoutCorrectAnswer
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

    }
}