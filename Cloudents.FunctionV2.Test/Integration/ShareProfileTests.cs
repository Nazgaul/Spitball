using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.FunctionsV2.Test.Integration
{
    public class ShareProfileTests
    {
        [Fact]
        public async Task CheckShareProfile()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://spitball-function-dev2.azurewebsites.net/api/share/profile/638");
            response.EnsureSuccessStatusCode();
        }
    }
}