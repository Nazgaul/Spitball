using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailDto
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public string Course { get; set; }
        public QuestionUserDto User { get; set; }
        public IEnumerable<QuestionDetailAnswerDto> Answers { get; set; }

        public DateTime Create { get; set; }

        public bool IsRtl { get; set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailAnswerDto
    {
        public QuestionDetailAnswerDto() { }


        public QuestionDetailAnswerDto(Guid id, string text, UserDto user,
            DateTime create,
            CultureInfo culture)
        {
            Id = id;
            Text = text;
            User = user;
            Create = create;

            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
        }



        public Guid Id { get; set; }

        public string Text { get; set; }

        public UserDto User { get; set; }
        public DateTime Create { get; set; }

        public bool IsRtl { get; set; }
    }
}
