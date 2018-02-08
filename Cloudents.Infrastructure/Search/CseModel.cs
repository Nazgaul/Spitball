using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Search
{
    public class SearchModel
    {
        public SearchModel(IEnumerable<string> query, IEnumerable<string> sources,
            SearchRequestSort sort, 
            CustomApiKey key, IEnumerable<string> courses, IEnumerable<string> universitySynonym, string defaultTerm, string docType)
        {
            Query = query;
            Sources = sources;

            //if (source != null)
            //{
            //    Source = string.Join(" OR ", source.Select(s => $"site:{s}"));
            //}
            Sort = sort;
            Key = key;
            Courses = courses;
            UniversitySynonym = universitySynonym;
            DefaultTerm = defaultTerm;
            DocType = docType;
        }

        public IEnumerable<string> Courses { get; }
        public IEnumerable<string> Query { get; }
        public IEnumerable<string> UniversitySynonym { get;  }

        public string DocType { get;  }

        public string DefaultTerm { get;  }

        public IEnumerable<string> Sources { get; }
        public SearchRequestSort Sort { get; }
        public CustomApiKey Key { get; }
    }
}