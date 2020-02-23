using System.Linq;
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

        [Theory]
        [InlineData("xxx", 0, "IL", 5)]
        [InlineData("Economics", 638, "IL", 5)]
        [InlineData("Economics", 0, "IL", 100)]
        [InlineData("Economics", 638, "IN", 5)]
        [InlineData("xxx", 0, "IN", 5)]

        public async Task TutorListByCourseQuery_Ok(string course, long userId, string country, int count)
        {

            var query = new TutorListByCourseQuery(course, userId, country, count);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            foreach (var tutorCardDto in result)
            {
                tutorCardDto.Country.Should().BeEquivalentTo(country);
            }
        }


        [Theory]
        [InlineData(0, "IL", 0)]
        [InlineData(0, "IN", 0)]
        [InlineData(638, null, 0)]

        public async Task TutorListQuery_Ok(long userId, string country, int page)
        {
            var query = new TutorListQuery(userId, country, page);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(638)]

        public async Task TutorListQuery_PageCountOk(long userId)
        {
            var query = new TutorListQuery(userId, "IL",0,int.MaxValue);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
            var count = result.Count;
            result.Count.Should().Be(result.Result.Count());

            var query2 = new TutorListQuery(userId, "IL", 0, 20);
            var result2 = await _fixture.QueryBus.QueryAsync(query2, default);
            result2.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(159039)]
        [InlineData(162107)]
        [InlineData(638)]
        public async Task TutorActionsQuery_Ok(long userId)
        {
            var query = new TutorActionsQuery(userId);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeNull();
        }
    }

    [Collection("Database collection")]
    public class TutorRepositoryReadTests
    {
        private readonly DatabaseFixture _fixture;

        public TutorRepositoryReadTests(DatabaseFixture fixture)
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


    [Collection("Database collection")]
    public class ReadTutorRepositoryReadTests
    {
        private readonly DatabaseFixture _fixture;

        public ReadTutorRepositoryReadTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetReadTutorAsync_Ok()
        {
            var res = await _fixture.ReadTutorRepository.GetReadTutorAsync(638, default);
            res.Should().NotBeNull();
        }
    }


}