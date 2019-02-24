using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

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
    }
}