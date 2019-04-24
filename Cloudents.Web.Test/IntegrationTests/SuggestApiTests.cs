using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SuggestApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public SuggestApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/Suggest?sentence=hi")]
        [InlineData("api/Suggest?sentence=hi&vertical=job")]
        [InlineData("api/Suggest?sentence=aj&vertical=tutor")]
        [InlineData("/api/suggest")]
        public async Task GetAsync_Ok(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}