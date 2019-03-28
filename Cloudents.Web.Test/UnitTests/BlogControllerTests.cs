using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    public class BlogControllerTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public BlogControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Redirect_Test()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            var response = await client.GetAsync("https://dev.spitball.co/blog");

            response.StatusCode.Should().Be(302);
            response.Headers.Location.Should().Be("https://medium.com/@spitballstudy");
        }
    }
}
