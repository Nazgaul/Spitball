using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Read
{
    public class SearchModel
    {
        public SearchModel(IEnumerable<string> query, IEnumerable<string> sources,
            CustomApiKey key, IEnumerable<string> courses,
            IEnumerable<string> universitySynonym,
             string docType)
        {
            Query = query?.Where(w => w != null);
            Sources = sources;
            Key = key;
            Courses = courses;
            UniversitySynonym = universitySynonym;
            DocType = docType;
        }

        public IEnumerable<string> Courses { get; }
        public IEnumerable<string> Query { get; }
        public IEnumerable<string> UniversitySynonym { get; }

        public string DocType { get; }


        public IEnumerable<string> Sources { get; }
        //public SearchRequestSort Sort { get; }
        public CustomApiKey Key { get; }
    }
}