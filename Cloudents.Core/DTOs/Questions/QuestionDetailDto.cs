using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs.Questions
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailDto
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public string Course { get; set; }
        public QuestionUserDto User { get; set; }
        public IList<QuestionDetailAnswerDto> Answers { get; set; }

        public DateTime Create { get; set; }

    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailAnswerDto
    {
        public QuestionDetailAnswerDto() { }


        public QuestionDetailAnswerDto(Guid id, string text, UserDto user,
            DateTime create)
        {
            Id = id;
            Text = text;
            User = user;
            Create = create;
        }



        public Guid Id { get; set; }

        public string Text { get; set; }

        public UserDto User { get; set; }
        public DateTime Create { get; set; }
    }
}
