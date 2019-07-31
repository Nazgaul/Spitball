using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper initialize")]
    public class QuestionsQuery //: VerticalQuery
    {
        public QuestionsQuery(//UserProfile userProfile, 

            string term,
            string course, 
            bool filterByUniversity,
            IEnumerable<QuestionFilter> filters, string country, Guid? universityId) //: base(userProfile, term, course, filterByUniversity)
        {
            
           // UserProfile = userProfile;
            Term = term;
            Course = course;
            FilterByUniversity = filterByUniversity;
            Filters = filters;
            Country = country;
            UniversityId = universityId;
        }


        //public UserProfile UserProfile { get; }
        public string Term { get; }
        public string Course { get; }
        public bool FilterByUniversity { get; }

        public IEnumerable<QuestionFilter> Filters { get; }

        public string Country { get; }
        public Guid? UniversityId { get; }

        public int Page { get; set; }
    }
}