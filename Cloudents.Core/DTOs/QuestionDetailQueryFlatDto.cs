using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class QuestionDetailQueryFlatDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int UserScore { get; set; }
        public long Id { get; set; }
        public string Text { get; set; }
        public decimal Price { get; set; }
        public DateTime Create { get; set; }
        public Guid? CorrectAnswerId { get; set; }
        public QuestionColor? Color { get; set; }
        public QuestionSubject Subject { get; set; }
        public string Language { get; set; }
        public int Votes { get; set; }
        public string Course { get; set; }
    }
}
