﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailDto
    {

        

        public QuestionSubject? Subject { get; set; }

        public long Id { get; set; }

        public string Text { get; set; }

        public decimal Price { get; set; }
        public string Course { get; set; }
        public UserDto User { get; set; }
        public IEnumerable<QuestionDetailAnswerDto> Answers { get; set; }

        public DateTime Create { get; set; }

        public IEnumerable<Uri> Files { get; set; }

        public Guid? CorrectAnswerId { get; set; }

        public bool IsRtl { get; set; }

        

        public VoteDto Vote { get; set; }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailAnswerDto
    {
        public QuestionDetailAnswerDto() { }


        public QuestionDetailAnswerDto(Guid id, string text, UserDto user, DateTime create, VoteDto vote,
            CultureInfo culture)
        {
            Id = id;
            Text = text;
            User = user;
            Create = create;
            Vote = vote;
            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
        }
       

       
        public Guid Id { get; set; }

        public string Text { get; set; }

        public UserDto User { get; set; }
        public DateTime Create { get; set; }

        public IEnumerable<Uri> Files { get; set; }

        public VoteDto Vote { get; set; }

        public bool IsRtl { get; set; }
    }
}
