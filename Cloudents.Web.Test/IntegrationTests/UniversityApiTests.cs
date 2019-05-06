using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UniversityApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;
        private readonly object uni = new
        {
            name = "Open University",
            country = "IL"
        };

        public UniversityApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_SomeLocation_Ok()
        {
            var response = await _client.GetAsync("api/university?Location.Longitude=-74.005&Location.Latitude=40.712");
            var result = await response.Content.ReadAsStringAsync();
            var d = JObject.Parse(result);
            var p = d["universities"].Values();
            p.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Post_Set_Uni()
        {
            var uniId = new
            {
                id = "bdb71a15-62ed-4fab-8a76-a98200e81a53"
            };

            await _client.LogInAsync();

            var response = await _client.PostAsync("api/university/set", HttpClient.CreateJsonString(uniId));

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/api/university")]
        public async Task GetAsync_OK(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var uni = d["universities"].Value<JArray>();

            var id = uni[0]["id"]?.Value<string>();
            var name = uni[0]["name"]?.Value<string>();
            var country = uni[0]["country"]?.Value<string>();

            id.Should().NotBeNull();
            name.Should().NotBeNull();
            country.Should().NotBeNull();
            uni.Should().HaveCountGreaterOrEqualTo(30);
        }

        [Theory]
        [InlineData("api/university?term=uni&page=0")]
        public async Task GetAsync_Paging(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var uni = d["universities"].Value<JArray>();

            var id = uni[0]["id"]?.Value<string>();

            uni.Should().NotBeNull();
        }

        [Fact(Skip = "this is not a good unit test - need to re-write it")]
        public async Task PostAsync_Create_Success()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/University/create", HttpClient.CreateJsonString(uni));

            response.EnsureSuccessStatusCode();
        }

        [Fact(Skip = "this is not a good unit test - need to re-write it")]
        public async Task PostAsync_Create_Failure()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/University/create", HttpClient.CreateJsonString(uni));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}