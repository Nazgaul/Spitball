using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class DocumentControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public DocumentControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("document/המסלול-האקדמי-המכללה-למנהל")]
        public async Task ShortUrl_Invalid_404(string url)
        {
            var response = await _client.GetAsync(url);
           
            var p = response.Headers.Location;
            p.Should().Be("/Error/NotFound");
            //Assert.EndsWith("error/notfound", p.AbsolutePath);
        }

        [Theory]
        [InlineData("api/document/2999")]
        public async Task Valid_Url_200(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}