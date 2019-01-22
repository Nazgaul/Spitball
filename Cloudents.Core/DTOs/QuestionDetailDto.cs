using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailDto
    {

        public QuestionDetailDto(UserDto user, long id, string text, decimal price,
             DateTime create,
            Guid? correctAnswerId,  QuestionSubject subject, 
            CultureInfo culture, int votes, string course)
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
            Course = course;
            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
            Answers = new List<QuestionDetailAnswerDto>();
        }

        [DtoToEntityConnection(nameof(Question.Subject))]
        public QuestionSubject Subject { get; }

        [DtoToEntityConnection(nameof(Question.Id))]
        public long Id { get; }

        [DtoToEntityConnection(nameof(Question.Text))]
        public string Text { get; }

        [DtoToEntityConnection(nameof(Question.Price))]
        public decimal Price { get; }
        [DtoToEntityConnection(nameof(Question.Course))]
        public string Course { get; set; }
        [DtoToEntityConnection(nameof(Question.User))]
        public UserDto User { get; }
        [DtoToEntityConnection(nameof(Question.Answers))]
        public List<QuestionDetailAnswerDto> Answers { get; set; }

        [DtoToEntityConnection(nameof(Question.Created))]
        public DateTime Create { get; }

        public IEnumerable<Uri> Files { get; set; }

        [DtoToEntityConnection(nameof(Question.CorrectAnswer))]
        public Guid? CorrectAnswerId { get; }

        //[DtoToEntityConnection(nameof(Question.Subject))]
        //public QuestionColor? Color { get; }

        [DtoToEntityConnection(nameof(Question.Language))]
        public bool IsRtl { get; }

        [DtoToEntityConnection(nameof(Question.Votes))]
        public VoteDto Vote { get;  }
    }

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Dto class")]
    public class QuestionDetailAnswerDto
    {
        public QuestionDetailAnswerDto() { }


        public QuestionDetailAnswerDto(Guid id, string text, UserDto user, DateTime create, VoteDto vote, CultureInfo culture)
        {
            Id = id;
            Text = text;
            User = user;
            Create = create;
            Vote = vote;
            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
        }
        public QuestionDetailAnswerDto(Guid id,
            string text, 
            long userId,
            string userName, 
            int userScore, 
            DateTime create, 
            int vote, 
            CultureInfo culture)
        {
            Id = id;
            Text = text;
            User = new UserDto(userId, userName, userScore);
            Create = create;
            Vote = new VoteDto { Votes = vote };
            IsRtl = culture?.TextInfo.IsRightToLeft ?? false;
        }

       
        [DtoToEntityConnection(nameof(Answer.Id))]
        public Guid Id { get; set; }

        [DtoToEntityConnection(nameof(Answer.Text))]
        public string Text { get; set; }

        [DtoToEntityConnection(nameof(Answer.User))]
        public UserDto User { get; set; }
        [DtoToEntityConnection(nameof(Answer.Created))]
        public DateTime Create { get; set; }

        public IEnumerable<Uri> Files { get; set; }

        [DtoToEntityConnection(nameof(Answer.Votes))]
        public VoteDto Vote { get; set; }
        [DtoToEntityConnection(nameof(Answer.Language))]
        public bool IsRtl { get; set; }

    }
}
