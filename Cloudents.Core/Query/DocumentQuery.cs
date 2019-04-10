using Cloudents.Core.Models;
using System.Collections.Generic;

namespace Cloudents.Core.Query
{
    public class DocumentQuery : VerticalQuery
    {
        public DocumentQuery(UserProfile userProfile, 
            string term,
            string course,
            bool filterByUniversity,
            IEnumerable<string> filters) :
            base(userProfile, term, course, filterByUniversity)
        {
            Filters = filters;
        }

        public IEnumerable<string> Filters { get; }

    }

    public abstract class VerticalQuery
    {


        protected VerticalQuery(UserProfile userProfile, string term, string course, bool filterByUniversity)
        {
            UserProfile = userProfile;
            Term = term;
            Course = course;
            if (string.IsNullOrEmpty(Course))
            {
                FilterByUniversity = filterByUniversity;
            }
            else
            {
                FilterByUniversity = true;
            }

            // Profile = profile;
        }

        public string Term { get; }
        public string Course { get; }
        public bool FilterByUniversity { get; }

        public int Page { get; set; }

        public UserProfile UserProfile { get;  }


    }
}