using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Admin.Test
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Admin2.Startup>>
    {
        private readonly WebApplicationFactory<Admin2.Startup> _factory;

        public BasicTests(WebApplicationFactory<Admin2.Startup> factory)
        {
            _factory = factory;
        }
        [Theory]
        [InlineData("/")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
