using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Models;

namespace Cloudents.Core.Query
{
    public class DocumentQuery
    {
        public DocumentQuery(string course, UserProfile profile, string term, int page,
            IEnumerable<string> filters)
        {
            Course = course;
            Term = term;
            Page = page;
            Filters = filters ?? Enumerable.Empty<string>();
            Profile = profile;
        }
        public string Course { get;  }


        public string Term { get; }
        public UserProfile Profile { get; }



        public int Page { get; }

        public IEnumerable<string> Filters { get; }

    }
}