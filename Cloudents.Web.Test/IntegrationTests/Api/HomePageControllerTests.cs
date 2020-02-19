using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class HomePageControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public HomePageControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Theory]
        [InlineData("api/HomePage/version")]
        [InlineData("api/HomePage/tutors")]
        [InlineData("api/HomePage/banner")]
        [InlineData("api/HomePage/reviews")]
        [InlineData("api/HomePage")]
        [InlineData("api/Homepage/documents")]
        public async Task GetAsync_HomePage_OkAsync(string uri)
        {
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var str = await response.Content.ReadAsStringAsync();
                str.IsValidJson().Should().BeTrue();
            }
        }
    }
}
