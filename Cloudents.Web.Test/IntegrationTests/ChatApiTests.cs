using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Net;
using System;
using Newtonsoft.Json.Linq;

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
            otherUser = "160171"
        };

        private readonly object _user = new
        {
            otherUser = "159039"
        };

        private readonly object _conv = new
        {
            userId = 0,
            name = "string",
            image = "string",
            unread = 0,
            online = true,
            conversationId = "string",
            dateTime = new DateTime(),
            studyRoomId = "string",
            lastMessage = "string"
        };



        public ChatApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Fact]
        public async Task GetAsync_Get_Chat()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Get_User_Chat()
        {
            await _client.LogInAsync();

            _uri.Path = "api/chat/159039";

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
            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path + "/1");

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            d.Should().NotBeNull();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Chat_Read_NoSuchConversation_BadRequest()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/read", HttpClient.CreateJsonString(_user));
           
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task PostAsync_Chat_Read_OK()
        {
            await _client.LogInAsync();

            _uri.Path = "api/chat";

            object msg = new
            {
                message = "Hi",
                otherUser = 160171
            };

            object otherUser = new
            {
                email = "elad12@cloudents.com",
                password = "123456789"
            };

            object read = new
            {
                otherUserId = 159039
            };


            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(msg));

            response.EnsureSuccessStatusCode();

            _uri.Path = "api/login";

            response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(otherUser));
            response.EnsureSuccessStatusCode();
            _uri.Path = "api/chat/read";

            response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(read));

            response.EnsureSuccessStatusCode();
        }
    }
}
