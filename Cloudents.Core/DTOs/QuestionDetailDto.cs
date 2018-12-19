using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Common.Enum;
using Cloudents.Domain.Enums;

namespace Cloudents.Application.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailDto
    {
        public QuestionDetailDto(UserDto user, long id, string text, decimal price,
             DateTime create,
            Guid? correctAnswerId, QuestionColor? color, QuestionSubject subject, 
            CultureInfo culture, int votes)
        {
            Subject = subject;
            Vote = new VoteDto
            {
                Votes = votes
            };
            Id = id;
            Text = text;
            Price = price;
            User = user;
            Create = create;
            CorrectAnswerId = correctAnswerId;
            Color = color;
            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
        }

        public QuestionSubject Subject { get; }

        public long Id { get; }

        public string Text { get; }

        public decimal Price { get; }

        public UserDto User { get; }
        public IEnumerable<QuestionDetailAnswerDto> Answers { get; set; }

        public DateTime Create { get; }

        public IEnumerable<Uri> Files { get; set; }

        public Guid? CorrectAnswerId { get; }

        public QuestionColor? Color { get; }

        public bool IsRtl { get; }

        public VoteDto Vote { get;  }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailAnswerDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public UserDto User { get; set; }
        public DateTime Create { get; set; }

        public IEnumerable<Uri> Files { get; set; }
        public VoteDto Vote { get; set; }

    }
}
