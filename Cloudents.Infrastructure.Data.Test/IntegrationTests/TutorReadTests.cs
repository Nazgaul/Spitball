using System.Threading.Tasks;
using Cloudents.Query.Query;
using Cloudents.Query.Tutor;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class TutorReadTests
    {
        private readonly DatabaseFixture _fixture;

        public TutorReadTests(DatabaseFixture fixture)
        {
            this._fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }

        [Fact]
        public async Task TutorListByCourseQuery_Ok()
        {
            var query = new TutorListByCourseQuery("xxx", 0);
            _ = await _fixture._queryBus.QueryAsync(query, default);


        }


        [Fact]
        public async Task TutorListQuery_Ok()
        {
            var query = new TutorListQuery(0);
            _ = await _fixture._queryBus.QueryAsync(query, default);


        }


        [Fact]
        public async Task TutorListTabSearchQuery_Ok()
        {
            var query = new TutorListTabSearchQuery("eidan", "IL", 0);
            _ = await _fixture._queryBus.QueryAsync(query, default);


        }

        [Fact]
        public async Task TutorListTabQuery_Ok()
        {
            var query = new TutorListTabQuery("IL", page: 0);
            _ = await _fixture._queryBus.QueryAsync(query, default);


        }
    }
}