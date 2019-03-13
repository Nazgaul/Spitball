using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json.Linq;
using FluentAssertions;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class BookApiTests : IClassFixture<SbWebApplicationFactory>
    {

        private readonly SbWebApplicationFactory _factory;

        public BookApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/book/search?term=kjhgfd")]
        [InlineData("api/book/search?term=%2Cmnhbg")]
        [InlineData("api/book/search?term=The%20medical")]
        [InlineData("/api/book/search?page=2&term=super mario 64 ds")]
        [InlineData("/api/book/buy?isbn13=9781292099170")]
        public async Task Search_ReturnResult(string url)
        {
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Fact]
        public async Task Search_Return_OK_Result()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/book/search?term=merk");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().HaveCount(9);
        }
    }
}