using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Controllers
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ProfileControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public ProfileControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }


        [Theory]
        [InlineData("en")]
        [InlineData("he")]
        [InlineData("en-IN")]
        public async Task GetAsync_Profile_OKAsync(string culture)
        {

            var response = await _client.GetAsync($"profile/638/Ram%20Yaari?culture={culture}");
            response.EnsureSuccessStatusCode();
        }
    }
}
