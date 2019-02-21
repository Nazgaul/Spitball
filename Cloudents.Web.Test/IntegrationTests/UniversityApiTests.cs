using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class UniversityApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UniversityApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAsync_SomeLocation_Ok()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("api/university?Location.Longitude=-74.005&Location.Latitude=40.712");
            var result = await response.Content.ReadAsStringAsync();
            var d = JObject.Parse(result);
            var p = d["universities"].Values();
            p.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}