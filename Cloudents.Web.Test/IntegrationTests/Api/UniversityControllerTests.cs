using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UniversityControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public UniversityControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }
        

        [Theory]
        [InlineData("/api/university")]
        [InlineData("api/university?term=אוני")]
        [InlineData("api/university?term=uni")]
        [InlineData("api/university?term=univer&page=1")]
        [InlineData("api/university?term=univer&page=0&country=&7BIN%7D")]
        public async Task GetAsync_University_OK(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}