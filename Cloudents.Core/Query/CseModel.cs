using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Core.Query
{
    public class SearchModel
    {
        public SearchModel(string query, IEnumerable<string> sources,
            CustomApiKey key, IEnumerable<string> courses,
            IEnumerable<string> universitySynonym)
        {
            Query = query;
            Sources = sources;
            Key = key;
            Courses = courses;
            UniversitySynonym = universitySynonym;
        }

        public IEnumerable<string> Courses { get; }
        public string Query { get; }

        [CanBeNull, ItemCanBeNull]
        public IEnumerable<string> UniversitySynonym { get; }

        [CanBeNull, ItemCanBeNull]
        public IEnumerable<string> Sources { get; }

        public CustomApiKey Key { get; }
    }
}