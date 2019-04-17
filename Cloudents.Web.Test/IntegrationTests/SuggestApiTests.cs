using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SuggestApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public SuggestApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/Suggest?sentence=hi")]
        [InlineData("api/Suggest?sentence=hi&vertical=job")]
        [InlineData("api/Suggest?sentence=aj&vertical=tutor")]
        [InlineData("/api/suggest")]
        public async Task GetAsync_Ok(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}