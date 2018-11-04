using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class QuestionFeedDto
    {
        public long Id { get; set; }
        public QuestionSubject Subject { get; set; }
        public decimal Price { get; set; }
        public string Text { get; set; }
        public int Files { get; set; }
        public int Answers { get; set; }
        public UserDto User { get; set; }

        public DateTime DateTime { get; set; }

        public QuestionColor? Color { get; set; }

        public bool HasCorrectAnswer { get; set; }
    }
}