using System;
using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{

    public class SearchWrapperDto <T>
    {
        public IEnumerable<T> Update { get; set; }
        public IEnumerable<string> Delete { get; set; }
        public long Version { get; set; }
    }

    public class QuestionSearchDto
    {
        [EntityBind(nameof(Question.Id))]
        public long Id { get; set; } //key readonly

        [EntityBind(nameof(Question.Updated))]
        public DateTime? DateTime { get; set; } //readonly

        [EntityBind(nameof(Question.Text))]
        public string Text { get; set; } //search readonly


        public string[] Prefix
        {
            get
            {
                var arr = new List<string>();
                if (!string.IsNullOrEmpty(Text))
                {
                    arr.Add(Text);
                }

                if (arr.Count == 0)
                {
                    return null;
                }

                return arr.ToArray();
            }
        } //search


        [EntityBind(nameof(Question.University.Country),nameof(Question.User.Country))]
        public string Country { get; set; }
       

        [EntityBind(nameof(Question.Language))]
        public string Language { get; set; }

        [EntityBind(nameof(Question.Course.Id))]
        public string Course { get; set; }


        //public QuestionSubject? Subject { get; set; } // facetable readonly
        [EntityBind(nameof(Question.CorrectAnswer),nameof(Question.Answers))]
        public QuestionFilter? State { get; set; }
        [EntityBind(nameof(Question.University.Name))]
        public string UniversityName { get; set; }
    }
}