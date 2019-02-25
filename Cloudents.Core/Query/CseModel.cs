//using System.Collections.Generic;
//using JetBrains.Annotations;

//namespace Cloudents.Core.Query
//{
//    public class SearchModel
//    {
//        public SearchModel(string query, IEnumerable<string> sources,
//            CustomApiKey key, string course,
//            IEnumerable<string> universitySynonym)
//        {
//            Query = query;
//            Sources = sources;
//            Key = key;
//            Course = course;
//            UniversitySynonym = universitySynonym;
//        }

//        public string Course { get; }
//        public string Query { get; }

//        [CanBeNull, ItemCanBeNull]
//        public IEnumerable<string> UniversitySynonym { get; }

//        [CanBeNull, ItemCanBeNull]
//        public IEnumerable<string> Sources { get; }

//        public CustomApiKey Key { get; }
//    }
//}