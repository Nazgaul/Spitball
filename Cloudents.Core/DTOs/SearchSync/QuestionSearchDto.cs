using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class QuestionSearchDto
    {
        private QuestionFilter? _filter;
        [DtoToEntityConnection(nameof(Question.Id))]
        public long QuestionId { get; set; } //key

        [DtoToEntityConnection(nameof(Question.Answers))]
        public int? AnswerCount { get; set; }
        [DtoToEntityConnection(nameof(Question.Created))]
        public DateTime? DateTime { get; set; }
        [DtoToEntityConnection(nameof(Question.CorrectAnswer))]
        public bool? HasCorrectAnswer { get; set; }
        [DtoToEntityConnection(nameof(Question.Text))]
        public string Text { get; set; }

        [DtoToEntityConnection(nameof(Question.User.Country))]
        public string Country { get; set; }
        [DtoToEntityConnection(nameof(Question.Language))]
        public string Language { get; set; }


        [DtoToEntityConnection(nameof(Question.Subject))]
        public QuestionSubject? Subject { get; set; } // facetable
        [DtoToEntityConnection(nameof(Question.Status.State))]
        public ItemState? State { get; set; }


        public QuestionFilter? Filter
        {
            get
            {
                if (_filter.HasValue)
                {
                    return _filter.Value;
                }
                var state = QuestionFilter.Unanswered;
                if (AnswerCount > 0)
                {
                    state = QuestionFilter.Answered;
                }

                if (HasCorrectAnswer.HasValue && HasCorrectAnswer.Value)
                {
                    state = QuestionFilter.Sold;
                }

                return state;
            }

            set => _filter = value;
        }
    }
}