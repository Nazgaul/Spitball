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


        public TutorApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_ReturnResult_OK()
        {
            _uri.Path = "api/tutor";

            var response = await _client.GetAsync(_uri.Path);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();

            var d = JArray.Parse(str);

            var result = d[0]?.Value<JObject>();

            result.Should().NotBeNull();
          
        }

        [Fact]
        public async Task GetAsync_Search_Without_Results()
        {
            _uri.Path = "api/tutor/search?term=gfc";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            str.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_OK_Result()
        {
            var response = await _client.GetAsync(_uri.Path);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact(Skip = "We did a hole markup change to tutor")]
        public async Task Post_Create_Room()
        {
            var response = await _client.PostAsync("api/tutoring/create", null);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["name"]?.Value<string>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_NonExist_Tutor()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path + "/search?term=fsdfds");
            
            var str = await response.Content.ReadAsStringAsync();
            
            var d = JObject.Parse(str);
            
            var result = d["result"]?.Value<JArray>();
            
            result.Should().BeEmpty();
        }
    }
}