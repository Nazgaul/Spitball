using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class UniversityTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UniversityTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/university")]
        public async Task GetAsync_OK(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var uni = d["universities"].Value<JArray>();

            var id = uni[0]["id"]?.Value<string>();
            var name = uni[0]["name"]?.Value<string>();
            var country = uni[0]["country"]?.Value<string>();

            id.Should().NotBeNull();
            name.Should().NotBeNull();
            country.Should().NotBeNull();
            uni.Should().HaveCountGreaterThan(5);
        }

        
    }


   
}