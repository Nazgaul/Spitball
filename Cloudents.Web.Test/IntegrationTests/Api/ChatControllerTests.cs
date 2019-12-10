using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ChatControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        //private readonly UriBuilder _uri = new UriBuilder()
        //{
        //    Path = "api/chat"
        //};

        private readonly object _msg = new
        {
            message = "string",
            otherUser = "160171"
        };

        private readonly object _user = new
        {
            otherUser = "159039"
        };

        //private readonly object _conv = new
        //{
        //    userId = 0,
        //    name = "string",
        //    image = "string",
        //    unread = 0,
        //    online = true,
        //    conversationId = "string",
        //    dateTime = new DateTime(),
        //    studyRoomId = "string",
        //    lastMessage = "string"
        //};




        public ChatControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        [Theory]
        [InlineData("api/chat")]
       
        public async Task ChatApiTestGet_NotLogIn_Unauthorized(string api)
        {
            var response = await _client.GetAsync(api);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("api/chat")]
        
        public async Task ChatApiTestGet_LogIn_Ok(string api)
        {
            await _client.LogInAsync();
            var response = await _client.GetAsync(api);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAsync_Get_Chat()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/chat");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Get_User_Chat()
        {
            await _client.LogInAsync();


            var response = await _client.GetAsync("api/chat/159039");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Send_Message()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/chat", HttpClient.CreateJsonString(_msg));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_NotValidUrl_Messages()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/chat/1");

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            d.Should().NotBeNull();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Chat_Read_NoSuchConversation_BadRequest()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/chat/read", HttpClient.CreateJsonString(_user));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task PostAsync_Chat_Read_OK()
        {
            await _client.LogInAsync();

            //_uri.Path = "api/chat";

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


            var response = await _client.PostAsync("api/chat", HttpClient.CreateJsonString(msg));

            response.EnsureSuccessStatusCode();

            //_uri.Path = "api/login";

            response = await _client.PostAsync("api/login", HttpClient.CreateJsonString(otherUser));
            response.EnsureSuccessStatusCode();
           // _uri.Path = "api/chat/read";

            response = await _client.PostAsync("api/chat/read", HttpClient.CreateJsonString(read));

            response.EnsureSuccessStatusCode();
        }
    }
}
