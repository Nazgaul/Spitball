using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class AccountControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly object _settings = new
        {
            firstName = "Tester",
            description = "I am a user for testing only",
            lastName = "User",
            bio = "Do not use this user for manual testing",
            price = 55
        };

        //private readonly UriBuilder _uri = new UriBuilder()
        //{
        //    Path = "api/account"
        //};

        public AccountControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/account")]
        [InlineData("api/account/referrals")]
        public async Task AccountApiTestGet_NotLogIn_UnauthorizedAsync(string api)
        {
            var response = await _client.GetAsync(api);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);            
        }

        [Theory]
        [InlineData("api/account")]
        [InlineData("api/account/referrals")]
        [InlineData("api/account/content")]
        [InlineData("api/account/purchases")]
        [InlineData("api/account/followers")]
        [InlineData("api/account/calendar")]
        [InlineData("api/account/stats?days=7")]
        [InlineData("api/account/stats?days=30")]
        [InlineData("api/account/stats?days=90")]
        [InlineData("api/account/tutorActions")]
        [InlineData("api/account/questions")]
        public async Task AccountApiTestGet_LogIn_OkAsync(string api)
        {
            await _client.LogInAsync();
            var response = await _client.GetAsync(api);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                var str = await response.Content.ReadAsStringAsync();
                str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
            }

            //response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        

        [Fact]
        public async Task PostAsync_settings_OKAsync()
        {
           //_uri.Path = "api/account/settings";

            await _client.LogInAsync();

            var response = await _client.PostAsync("api/account/settings", HttpClientExtensions.CreateJsonString(_settings));

            response.EnsureSuccessStatusCode();

           // _uri.Path = "api/profile/159489";

            response = await _client.GetAsync("api/profile/159489");
            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var tutor = d["tutor"]?.Value<JObject>() ?? throw new ArgumentNullException("d[\"tutor\"]?.Value<JObject>()");

            var firstName = d["firstName"]?.Value<string>();
            var lastName = d["lastName"]?.Value<string>();
            var price = tutor["price"]?.Value<decimal>();


            firstName.Should().Be("Tester");
            lastName.Should().Be("User");
            price.Should().Be(55M);
        }

        [Fact]
        public async Task GetAsync_sales_OKAsync()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/Sales/sales");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStringAsync();

            dynamic v = JsonConvert.DeserializeObject(responseBody);

            /*string course = v[0].course;
            course.Should().NotBeNull();

            string name = v[0].name;
            name.Should().NotBeNull();

            long id = v[0].id;
            id.Should().NotBe(0);

            string preview = v[0].preview;
            preview.Should().NotBeNull();

            string url = v[0].url;
            url.Should().NotBeNull();*/

            string paymentStatus = v[0].paymentStatus;
            paymentStatus.Should().NotBeNull();

            string type = v[0].type;
            type.Should().NotBeNull();

            string date = v[0].date;
            date.Should().NotBeNull();

            int price = v[0].price;
            price.Should().BeGreaterOrEqualTo(0);
        }

    }
}
