using Cloudents.Core.Enum;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public SearchQuery(string[] query, string universitySynonym, 
            string course, string source, int page, SearchCseRequestSort sort)
        {
            Query = query;
            UniversitySynonym = universitySynonym;
            Course = course;
            Source = source;
            Page = page;
            Sort = sort;
        }

        public string Source { get;  }
        public string UniversitySynonym { get;  }
        public string Course { get;  }
        public string[] Query { get;  }
        public int Page { get;  }
        public SearchCseRequestSort Sort { get;  }
    }
}