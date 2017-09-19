using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public SearchQuery(string[] query, string universitySynonym, string course, string source, int page, SearchRequestSort sort)
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
        public SearchRequestSort Sort { get;  }
    }

  


    //public class SearchRequest
    //{
    //    public string Source { get; set; }
    //    public long? University { get; set; }
    //    public string Course { get; set; }
    //    public string[] Query { get; set; }
    //    public int Page { get; set; }
    //    public SearchRequestSort Sort { get; set; }
    //}
}