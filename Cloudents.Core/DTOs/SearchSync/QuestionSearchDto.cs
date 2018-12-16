using Cloudents.Core.Entities.Search;
using System;
using Cloudents.Core.Enum;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class QuestionSearchDto
    {
        private QuestionFilter? _filter;
        public long QuestionId { get; set; } //key

        public int AnswerCount { get; set; }
        public DateTime DateTime { get; set; }
        public bool HasCorrectAnswer { get; set; }
        public string Text { get; set; }

        public string Country { get; set; }
        public string Language { get; set; }


        public Cloudents.Common.QuestionSubject Subject { get; set; } // facetable
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

                if (HasCorrectAnswer)
                {
                    state = QuestionFilter.Sold;
                }

                return state;
            }

            set => _filter = value;
        }
    }
}