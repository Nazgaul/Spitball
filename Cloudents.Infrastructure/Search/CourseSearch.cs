//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities.Search;
//using Cloudents.Core.Interfaces;
//using Cloudents.Infrastructure.Write;
//using JetBrains.Annotations;
//using Microsoft.Azure.Search;
//using Microsoft.Azure.Search.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Query;

//namespace Cloudents.Infrastructure.Search
//{
//    [UsedImplicitly]
//    public class CourseSearch : ICourseSearch
//    {
//        private readonly ISearchIndexClient _client;

//        public CourseSearch(ISearchService client)
//        {
//            _client = client.GetClient(CourseSearchWrite.IndexName);
//        }

//        public async Task<IEnumerable<CourseDto>> SearchAsync(CourseSearchQuery query, 
//            CancellationToken token)
//        {
//            var term = query.Term;
//            term = term?.Replace(":", @"\:");

//            var result = await _client.Documents.SearchAsync<Course>(term, new SearchParameters
//            {
//                Select = new[] { nameof(Course.Name) },
//                Top = 10,
//                ScoringProfile = CourseSearchWrite.ScoringProfile

//            }, cancellationToken: token);

//            return result.Results.Select(s => new CourseDto()
//            {
//                Name = s.Document.Name
//            });
//        }
//    }
//}
