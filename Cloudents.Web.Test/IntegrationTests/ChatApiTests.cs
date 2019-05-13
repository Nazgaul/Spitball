using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net;
using System;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ChatApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly UriBuilder _uri = new UriBuilder()
        {
            Path = "api/chat"
        };

        private readonly object _msg = new
        {
            message = "string",
            otherUser = "160116"
        };

        private readonly object _user = new
        {
            otherUser = "159039"
        };



        public ChatApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_Get_Chat()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Send_Message()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_msg));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_NotValidUrl_Messages()
        {   
            var response = await _client.GetAsync(_uri.Path + "/159039");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostAsyncChatRead_NoSuchConversation_BadRequest()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/read", HttpClient.CreateJsonString(_user));
           
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
    }
}
