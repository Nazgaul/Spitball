using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TutorApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/tutor"
        };

        public object HttpsStatusCode { get; private set; }

        public TutorApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        
        [Theory(Skip = "We did a hole markup change to tutor")]
        [InlineData("/api/Tutor?term=ajax&sort=null&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("/api/Tutor?term=ajax&sort=price&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("/api/Tutor?term=ajax&sort=relevance&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("api/Tutor?term=ajax&sort=price")]
        [InlineData("api/Tutor")]
        public async Task Search_ReturnResult(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"].Value<JArray>();

            response.EnsureSuccessStatusCode();

            result.Should().NotBeNull();
          
        }

        [Fact]
        public async Task Get_OK_Result()
        {
            var response = await _client.GetAsync(_uri.Path);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(Skip = "We did a hole markup change to tutor")]
        public async Task Get_Empty_Result()
        {
            var response = await _client.GetAsync("api/tutor?term=gfsd");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().BeEmpty();
        }

        [Fact(Skip = "We did a hole markup change to tutor")]
        public async Task Post_Create_Room()
        {
            var response = await _client.PostAsync("api/tutoring/create", null);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["name"]?.Value<string>();

            response.StatusCode.Should().Be(200);

            result.Should().NotBeNull();
        }
    }
}