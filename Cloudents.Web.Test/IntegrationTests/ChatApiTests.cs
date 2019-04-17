using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class ChatApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public ChatApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAsync_Get_Chat()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.GetAsync("api/Chat");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Send_Message()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string msg = "{\"message\":\"string\",\"otherUser\":160116}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Chat", new StringContent(msg, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetAsync_Chat_Messages()
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync("api/Chat/159039");

            var str = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_Chat_Read_Messages()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Chat/read", new StringContent("{\"otherUser\":159039}", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
    }
}
