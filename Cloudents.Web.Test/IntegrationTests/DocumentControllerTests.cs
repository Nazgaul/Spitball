using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class DocumentControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public DocumentControllerTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("document/המסלול-האקדמי-המכללה-למנהל")]
        public async Task ShortUrl_Invalid_404(string url)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });

            // Act
            var response = await client.GetAsync(url);
           
            var p = response.Headers.Location;
            p.Should().Be("/Error/NotFound");
            //Assert.EndsWith("error/notfound", p.AbsolutePath);
        }

        [Theory]
        [InlineData("document/2999")]
        public async Task Valid_Url_200(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            response.StatusCode.Should().Be(200);
        }
    }
}