using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Query;
using Cloudents.Query.Documents;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Xunit;
using FluentAssertions;

namespace Cloudents.Search.Test
{
    [Collection("Database collection")]
    public class DocumentSearchTest
    {
        public DocumentSearchTest(SearchFixture fixture)
        {
            _client = fixture.DocumentsSearch;
            _queryBus = fixture.QueryBus;
        }
        private readonly IDocumentsSearch _client;
        private readonly IQueryBus _queryBus;

        [Theory]
        [InlineData("IL", null, "משפטים - דיני עונשין", null)]
        [InlineData("IL", null, "משפטים - דיני עונשין", DocumentType.Video)]
        [InlineData("IL", null, "משפטים - דיני עונשין", DocumentType.Document)]
        public async Task DocumentSearch_Params_Ok(string country, string term, string course, DocumentType? filter)
        {
            var userProfile = new UserProfile()
            {
                Country = country
            };
            var query = new DocumentQuery(userProfile, term, course, 0, 20, filter);
            var searchResult = await _client.SearchAsync(query, userProfile, default);

            var documentResult = searchResult.result.ToList();
            var ids = documentResult.Select(s => s.Id);
            var queryDb = new IdsDocumentsQuery(ids);
            var dbResult = await _queryBus.QueryAsync(queryDb, default);
            if (dbResult.Count > 0)
            {
                dbResult.Should().OnlyContain(o => o.Course == course);
            }
        }


    }
}
