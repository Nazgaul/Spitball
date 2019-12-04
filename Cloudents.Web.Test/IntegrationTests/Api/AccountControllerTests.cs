using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class AccountControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly object _settings = new
        {
            firstName = "Skyler",
            description = "Nice to meet you",
            lastName = "White",
            bio = "I have a lot of experience",
            price = 55
        };

        //private readonly UriBuilder _uri = new UriBuilder()
        //{
        //    Path = "api/account"
        //};

        public AccountControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Theory]
        [InlineData("api/account")]
        [InlineData("api/account/courses")]
        [InlineData("api/account/university")]
        [InlineData("api/account/referrals")]
        public async Task AccountApiTestGet_NotLogIn_Unauthorized(string api)
        {
            var response = await _client.GetAsync(api);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("api/account")]
        [InlineData("api/account/courses")]
        [InlineData("api/account/university")]
        [InlineData("api/account/referrals")]
        public async Task AccountApiTestGet_LogIn_Ok(string api)
        {
            await _client.LogInAsync();
            var response = await _client.GetAsync(api);
            response.EnsureSuccessStatusCode();
            //response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        //[Fact]
        //public async Task GetAsync_Unauthorized_401()
        //{
        //    var response = await _client.GetAsync("api/account");

        //    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        //}

        //[Fact]
        //public async Task GetAsync_OK_200()
        //{
        //    await _client.LogInAsync();

        //    var response = await _client.GetAsync("api/account");

        //    response.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task GetAsync_courses_OK()
        //{
        //   // _uri.Path = "api/account/courses";

        //    await _client.LogInAsync();

        //    var response = await _client.GetAsync("api/account/courses");
        //    response.EnsureSuccessStatusCode();
        //    //var str = await response.Content.ReadAsStringAsync();

        //    //var d = JArray.Parse(str);

        //    //response.EnsureSuccessStatusCode();

        //    //d.Should().NotBeNull();
        //}

        //[Fact]
        //public async Task GetAsync_courses_Unauthorized()
        //{
        //   // _uri.Path = "api/account/courses";
        //    var response = await _client.GetAsync("api/account/courses");
        //    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        //}

        [Fact(Skip = "Need Redu")]
        public async Task PostAsync_settings_OK()
        {
           //_uri.Path = "api/account/settings";

            await _client.LogInAsync();

            var response = await _client.PostAsync("api/account/settings", HttpClient.CreateJsonString(_settings));

            response.EnsureSuccessStatusCode();

           // _uri.Path = "api/profile/159489";

            response = await _client.GetAsync("api/profile/159489");
            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var tutor = d["tutor"]?.Value<JObject>() ?? throw new ArgumentNullException("d[\"tutor\"]?.Value<JObject>()");

            var firstName = tutor["firstName"]?.Value<string>();
            var lastName = tutor["lastName"]?.Value<string>();
            var price = tutor["price"]?.Value<decimal>();


            firstName.Should().Be("Skyler");
            lastName.Should().Be("White");
            price.Should().Be(55M);
        }
    }
}
