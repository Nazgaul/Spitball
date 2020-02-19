using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Controllers
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class DocumentControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public DocumentControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }


        //private UriBuilder _uri = new UriBuilder()
        //{
        //    Path = string.Empty
        //};

        [Fact]
        public async Task GetAsync_OldDocument_OKAsync()
        {
            var response = await _client.GetAsync("document/Box%20Read%20for%20hotmail%20user/Load%20Stress%20Testing%20Multimi2.docx/457");
            response.EnsureSuccessStatusCode();
        }


        [Theory]
        [InlineData("en")]
        [InlineData("he")]
        [InlineData("en-IN")]
        public async Task GetAsync_Document_OKAsync(string culture)
        {

            var response = await _client.GetAsync($"document/פריוריטי-פיתוח/פריוריטי-בניית-דוחות/22?culture={culture}");
            response.EnsureSuccessStatusCode();
        }
    }
}
