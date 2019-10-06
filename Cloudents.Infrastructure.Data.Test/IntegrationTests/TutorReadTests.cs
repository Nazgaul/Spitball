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
        [InlineData("xxx", 0, 5)]
        [InlineData("Economics", 638, 5)]

        public async Task TutorListByCourseQuery_Ok(string course, long userId, int count)
        {
            var query = new TutorListByCourseQuery(course, userId, count);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);


        }


        [Theory]
        [InlineData(0,"IL",0)]
        [InlineData(638,null,0)]

        public async Task TutorListQuery_Ok(long userId, string country, int page)
        {
            var query = new TutorListQuery(userId, country, page);
            var result = await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeEmpty();

         

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