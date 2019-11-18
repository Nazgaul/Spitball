using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Query;
using Cloudents.Query.Documents;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        [InlineData("IL", null, "Economics")]
        public async Task Test1(string country, string term, string course, CancellationToken token = default)
        {
            var userprofile = new UserProfile()
            {
                Country = country
            };
            var query = new DocumentQuery(userprofile, term, course, 20, null);
            var searchResult = await _client.SearchAsync(query, userprofile, token);

            var documentResult = searchResult.result.ToList();
            var ids = documentResult.Select(s => s.Id);
            var queryDb = new IdsDocumentsQuery(ids);
            var dbResult = await _queryBus.QueryAsync(queryDb, token);

            dbResult.Should().OnlyContain(o => o.Course == course);
        }


    }
}
