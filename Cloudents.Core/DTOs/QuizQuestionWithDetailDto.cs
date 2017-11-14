using System;
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class QuizQuestionWithDetailDto
    {
        public QuizQuestionWithDetailDto()
        {
            Answers = new List<QuizAnswerWithDetailDto>();
        }
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid? CorrectAnswer { get; set; }

        public List<QuizAnswerWithDetailDto> Answers { get; set; }
    }
}