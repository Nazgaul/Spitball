using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailDto
    {
        public string Subject { get; set; }

        public long Id { get; set; }

        public string Text { get; set; }

        public decimal Price { get; set; }

        public UserDto User { get; set; }
        public IEnumerable<QuestionDetailAnswerDto> Answers { get; set; }

        public DateTime Create { get; set; }

        public IEnumerable<Uri> Files { get; set; }

        public Guid? CorrectAnswerId { get; set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailAnswerDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public UserDto User { get; set; }
        public DateTime Create { get; set; }

        public IEnumerable<Uri> Files { get; set; }
    }
}
