using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
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
        [InlineData("api/HomePage/reviews")]
        [InlineData("api/HomePage")]
        [InlineData("api/Homepage/documents")]
        public async Task GetAsync_HomePage_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);
            
            response.EnsureSuccessStatusCode();
        }
    }
}
