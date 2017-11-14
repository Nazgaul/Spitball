using System;

namespace Cloudents.Core.DTOs
{
    public class QuizAnswerWithDetailDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Guid QuestionId { get; set; }
    }
}