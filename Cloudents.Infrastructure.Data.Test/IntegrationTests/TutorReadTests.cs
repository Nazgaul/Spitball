using Cloudents.Query.Tutor;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    [Collection("Database collection")]
    public class TutorReadTests
    {
        private readonly DatabaseFixture _fixture;

        public TutorReadTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }

        [Fact]
        public async Task TutorListByCourseQuery_Ok()
        {
            var query = new TutorListByCourseQuery("xxx", 0, 5);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);


        }


        [Fact]
        public async Task TutorListQuery_Ok()
        {
            var query = new TutorListQuery(0, "IL",0);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeEmpty();

            var query2 = new TutorListQuery(638, null, 0);
            result = await _fixture.QueryBus.QueryAsync(query2, default);
            result.Should().NotBeEmpty();

        }

        
  
    }

    [Collection("Database collection2")]
    public class TutorRepositoryReadTests
    {
        private readonly DatabaseFixture2 _fixture;

        public TutorRepositoryReadTests(DatabaseFixture2 fixture)
        {
            _fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }

        [Fact]
        public async Task GetTutorsByCourseAsync_Ok()
        {
            var res = await _fixture.TutorRepository.GetTutorsByCourseAsync("פיזיקה 2", 161238, "IL", default);
            res.Should().NotBeEmpty();
        }
    }
}