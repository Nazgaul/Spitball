using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ChatApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public ChatApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_Get_Chat()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/Chat");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Send_Message()
        {
            var msg = new
            {
                message = "string",
                otherUser = "160116"
            };

            await _client.LogInAsync();

            var response = await _client.PostAsync("api/Chat", HttpClient.CreateJsonString(msg));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_NotValidUrl_Messages()
        {   
            var response = await _client.GetAsync("api/Chat/159039");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostAsyncChatRead_NoSuchConversation_BadRequest()
        {
            var user = new
            {
                otherUser = "159039"
            };

            await _client.LogInAsync();

            var response = await _client.PostAsync("api/Chat/read", HttpClient.CreateJsonString(user));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
    }
}
