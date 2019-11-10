using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UniversityApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly object _university = new
        {
            name = "Open University",
            country = "IL"
        };

        private readonly object _uniId = new
        {
            id = "bdb71a15-62ed-4fab-8a76-a98200e81a53"
        };

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/university"
        };


        public UniversityApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Fact]
        public async Task GetAsync_SomeLocation_Ok()
        {
            var response = await _client.GetAsync(_uri.Path + "?Location.Longitude=-74.005&Location.Latitude=40.712");

            var result = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(result);

            var p = d["universities"].Values();

            p.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Post_Set_Uni()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/set", HttpClient.CreateJsonString(_uniId));

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

            //var id = uni[0]["id"]?.Value<string>();

            uni.Should().NotBeNull();
        }

        [Fact(Skip = "this is not a good unit test - need to re-write it")]
        public async Task PostAsync_Create_Success()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/create", HttpClient.CreateJsonString(_university));

            response.EnsureSuccessStatusCode();
        }

        [Fact(Skip = "this is not a good unit test - need to re-write it")]
        public async Task PostAsync_Create_Failure()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/create", HttpClient.CreateJsonString(_university));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_University()
        {
            await _client.LogInAsync();

            _uri.Path = "api/university";

            var response = await _client.GetAsync(_uri.Path);

            response.Should().NotBeNull();
        }
    }
}