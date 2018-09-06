using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class QuestionWithoutCorrectAnswerDto
    {

        public long QuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public string  QuestionText { get; set; }
        public string AnswerText { get; set; }

        public string Url { get; set; }

        public bool IsFictive { get; set; }

    }
}