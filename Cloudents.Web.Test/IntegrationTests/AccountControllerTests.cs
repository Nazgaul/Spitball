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

        private readonly UriBuilder _uri = new UriBuilder()
        {
            Path = "api/account"
        };

        public AccountControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Fact]
        public async Task GetAsync_Unauthorized_401()
        {            
            var response = await _client.GetAsync(_uri.Path);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAsync_OK_200()
        {
            await _client.LogInAsync();
            
            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_courses_OK()
        {
            _uri.Path = "api/account/courses";

            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            response.EnsureSuccessStatusCode();

            d.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_courses_Unauthorized()
        {
            _uri.Path = "api/account/courses";

            var response = await _client.GetAsync(_uri.Path);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostAsync_settings_OK()
        {
            _uri.Path = "api/account/settings";

            object settings = new
            {
                firstName = "Elad",
                description = "Nice to meet you",
                lastName = "Levavi",
                bio = "I have a lot of experience",
                price = 55
            };

            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(settings));

            response.EnsureSuccessStatusCode();

            _uri.Path = "api/profile/159039";

            response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var tutor = d["tutor"]?.Value<JObject>();

            var firstName = tutor["firstName"]?.Value<String>();
            var lastName = tutor["lastName"]?.Value<String>();
            var description = d["description"]?.Value<String>();
            //var bio = tutor["bio"]?.Value<String>();
            var price = tutor["price"]?.Value<Decimal>();

            firstName.Should().Be("Elad");
            lastName.Should().Be("Levavi");
            description.Should().Be("Nice to meet you");
            //bio.Should().Be("I have a lot of experience");
            price.Should().Be(55M);

            response.EnsureSuccessStatusCode();
        }
    }
}
