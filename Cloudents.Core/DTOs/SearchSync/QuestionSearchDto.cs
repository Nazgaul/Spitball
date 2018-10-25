using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class QuestionSearchDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }


        public string Id { get; set; } //key

        public int AnswerCount { get; set; }
        public DateTime DateTime { get; set; }
        public int FilesCount { get; set; }
        public bool HasCorrectAnswer { get; set; }
        public double Price { get; set; }
        public string Text { get; set; }


        public string Country { get; set; }
        public Guid? UniversityId { get; set; }
        public string Language { get; set; }

        public QuestionColor Color { get; set; }

        public QuestionSubject Subject { get; set; } // facetable
        public QuestionState? State { get; set; }

        public Question ToQuestion()
        {
            var state = QuestionFilter.Unanswered;
            if (AnswerCount > 0)
            {
                state = QuestionFilter.Answered;
            }

            if (HasCorrectAnswer)
            {
                state = QuestionFilter.Sold;
            }

            return new Question()
            {
                Id = Id,
                DateTime = DateTime,
                Country = Country,
                UserId = UserId,
                UserName = UserName,
                Subject = Subject,
                AnswerCount = AnswerCount,
                HasCorrectAnswer = HasCorrectAnswer,
                FilesCount = FilesCount,
                Text = Text,
                UserImage = UserImage,
                Price = Price,
                Color = Color,
                State = state,
                Prefix = new[] { Text },
                Language = Language,
                UniversityId = UniversityId?.ToString()
            };
        }
    }
}