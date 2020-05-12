using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Query;
using Cloudents.Query.Documents;
using FluentAssertions;
using Xunit;

namespace Cloudents.Search.Test
{
    [Collection("Database collection")]
    public class TutorSearchTest
    {
        public TutorSearchTest(SearchFixture fixture)
        {
            _client = fixture.TutorSearch;
            _queryBus = fixture.QueryBus;
        }
        private readonly ITutorSearch _client;
        private readonly IQueryBus _queryBus;

        [Theory]
        [InlineData("IL", "a")]
        [InlineData("US", "b")]
        [InlineData("IN", "C")]
        public async Task TutorSearch_Params_Ok(string country, string term)
        {
            var userProfile = new UserProfile()
            {
                Country = country
            };
            var query = new TutorListTabSearchQuery(term, userProfile.CountryRegion, 0, 20);
            var searchResult = await _client.SearchAsync(query, default);

            //var documentResult = searchResult.result.ToList();
            //var ids = documentResult.Select(s => s.Id);
            //var queryDb = new IdsDocumentsQuery(ids);
            //var dbResult = await _queryBus.QueryAsync(queryDb, default);
            //if (dbResult.Count > 0)
            //{
            //    dbResult.Should().OnlyContain(o => o.Course == course);
            //}
        }
    }
}