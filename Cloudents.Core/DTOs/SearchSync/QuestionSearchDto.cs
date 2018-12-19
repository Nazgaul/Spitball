using System;
using Cloudents.Application.Enum;
using Cloudents.Common.Enum;
using Cloudents.Domain.Enums;

namespace Cloudents.Application.DTOs.SearchSync
{
    public class QuestionSearchDto
    {
        private QuestionFilter? _filter;
        public long QuestionId { get; set; } //key

        public int? AnswerCount { get; set; }
        public DateTime? DateTime { get; set; }
        public bool? HasCorrectAnswer { get; set; }
        public string Text { get; set; }

        public string Country { get; set; }
        public string Language { get; set; }


        public QuestionSubject? Subject { get; set; } // facetable
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