using System;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class QuestionSearchDto
    {

        public long Id { get; set; } //key readonly

        public DateTime? DateTime { get; set; } //readonly

        public string Text { get; set; } //search readonly


        public string[] Prefix
        {
            get
            {
                if (Text != null && Subject != null)
                {
                    return new[] { Text }.Union(Subject.GetEnumLocalizationAllValues()).ToArray();
                }

                return null;
            }
        } //search


        public string Country { get; set; }
       

        public string Language { get; set; }

        public string Course { get; set; }


        public QuestionSubject? Subject { get; set; } // facetable readonly
        public QuestionFilter? State { get; set; }


        //private QuestionFilter? _filter;
        //public long QuestionId { get; set; } //key

        //public int? AnswerCount { get; set; }
        //public DateTime? DateTime { get; set; }
        //public bool? HasCorrectAnswer { get; set; }
        //public string Text { get; set; }

        //public string Country { get; set; }
        //public string Language { get; set; }


        //public QuestionSubject? Subject { get; set; } // facetable
        //public ItemState? State { get; set; }


        //public QuestionFilter? Filter
        //{
        //    get
        //    {
        //        if (_filter.HasValue)
        //        {
        //            return _filter.Value;
        //        }
        //        var state = QuestionFilter.Unanswered;
        //        if (AnswerCount > 0)
        //        {
        //            state = QuestionFilter.Answered;
        //        }

        //        if (HasCorrectAnswer.HasValue && HasCorrectAnswer.Value)
        //        {
        //            state = QuestionFilter.Sold;
        //        }

        //        return state;
        //    }

        //    set => _filter = value;
        //}
    }
}