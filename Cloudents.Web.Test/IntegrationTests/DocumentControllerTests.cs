using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class DocumentControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public DocumentControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/item/המרכז-האקדמי-לב-מכון-טל/25405/סימולציה/401065/consoleapplication1.xml")]
        public async Task LinkToHeb_RedirectResult(string url)
        {
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            var p = response.Headers.Location;
            Assert.EndsWith("/", p.AbsolutePath);
        }
    }
}