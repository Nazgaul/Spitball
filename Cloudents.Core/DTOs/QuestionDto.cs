using System;
using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class QuestionDto
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


    public class QuestionDtoEqualityComparer : IEqualityComparer<QuestionDto>
    {
        public bool Equals(QuestionDto x, QuestionDto y)
        {
            return y != null && (x != null && x.Id == y.Id);
        }

        public int GetHashCode(QuestionDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}