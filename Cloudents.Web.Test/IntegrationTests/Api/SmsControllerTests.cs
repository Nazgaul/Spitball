using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SmsControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public SmsControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Theory]
        [InlineData("api/sms/code")]
        public async Task GetAsync_Sms_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
        }
    }
}
