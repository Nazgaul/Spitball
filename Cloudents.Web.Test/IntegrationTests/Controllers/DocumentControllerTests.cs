using Microsoft.AspNetCore.Mvc.Testing;
using System;
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


        private UriBuilder _uri = new UriBuilder()
        {
            Path = string.Empty
        };

        [Fact]
        public async Task GetAsync_OldDocument_OKAsync()
        {
            _uri.Path = "document/Box%20Read%20for%20hotmail%20user/Load%20Stress%20Testing%20Multimi2.docx/457";

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();
        }
    }
}
