using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    
    public class TutorApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public TutorApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Tutor?term=ajax&sort=null&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("/api/Tutor?term=ajax&sort=price&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        public async Task Search_ReturnResult(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
          
        }
    }
}