using System.Threading.Tasks;
using Cloudents.Query.Tutor;
using FluentAssertions;
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
            var query = new TutorListByCourseQuery("xxx", 0);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);


        }


        [Fact]
        public async Task TutorListQuery_Ok()
        {
            var query = new TutorListQuery(638,"IL");
            var result= await _fixture.QueryBus.QueryAsync(query, default);
            result.Should().NotBeEmpty();
            var query2 = new TutorListQuery(638, null);
             result = await _fixture.QueryBus.QueryAsync(query2, default);

            result.Should().NotBeEmpty();

        }


     

        [Fact]
        public async Task TutorListTabQuery_Ok()
        {
            var query = new TutorListTabQuery("IL", page: 0);
            var _ = await _fixture.QueryBus.QueryAsync(query, default);


        }
    }
}