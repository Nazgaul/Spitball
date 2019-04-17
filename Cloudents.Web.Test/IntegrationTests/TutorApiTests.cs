using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TutorApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public TutorApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/Tutor?term=ajax&sort=null&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("/api/Tutor?term=ajax&sort=price&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("/api/Tutor?term=ajax&sort=relevance&page=0&location.latitude=31.9155609&location.longitude=34.8049517")]
        [InlineData("api/Tutor?term=ajax&sort=price")]
        [InlineData("api/Tutor")]
        public async Task Search_ReturnResult(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"].Value<JArray>();

            response.EnsureSuccessStatusCode();
            result.Should().NotBeNull();
          
        }

        [Fact]
        public async Task Get_OK_Result()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/tutor");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var filters = d["filters"]?.Value<JArray>();
            var id = filters[0]["id"]?.Value<string>();
            id.Should().Be("Filter");
            var title = filters[0]["title"]?.Value<string>();
            title.Should().Be("Status");
            var data = filters[0]["data"]?.Value<JArray>();
            var key = data[0]["key"]?.Value<string>();
            var value = data[0]["value"]?.Value<string>();
            key.Should().Be("Online");
            value.Should().Be("Online");
            key = data[1]["key"]?.Value<string>();
            value = data[1]["value"]?.Value<string>();
            key.Should().Be("InPerson");
            value.Should().Be("InPerson");

            id.Should().NotBeNull();
            title.Should().NotBeNull();
            data.Should().NotBeNull();

            var sort = d["sort"]?.Value<JArray>();
            key = sort[0]["key"]?.Value<string>();
            value = sort[0]["value"]?.Value<string>();
            key.Should().Be("Relevance");
            value.Should().Be("Relevance");
            key = sort[1]["key"]?.Value<string>();
            value = sort[1]["value"]?.Value<string>();
            key.Should().Be("Price");
            value.Should().Be("Price");
        }

        [Fact]
        public async Task Get_Empty_Result()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/tutor?term=gfsd");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Post_Create_Room()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync("api/tutoring/create", null);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["name"]?.Value<string>();

            response.StatusCode.Should().Be(200);
            result.Should().NotBeNull();
        }
    }
}