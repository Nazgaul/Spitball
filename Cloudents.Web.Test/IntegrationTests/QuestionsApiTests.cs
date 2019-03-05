using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class QuestionsApiTests :  IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public QuestionsApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAsync_Filters()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/question");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();
            var subject = filters[1]["data"]?.Value<JArray>();

            filters.Should().HaveCount(2);
            type.Should().HaveCount(3);
            subject.Should().HaveCount(24);
        }

        [Theory]
        [InlineData("/api/Question?term=javascript:alert(219)")]
        [InlineData("/api/Question?term=main() { int a%3D4%2Cb%3D2%3B a%3Db<<a %2B b>>2%3B printf(\"%25d\"%2C a)%3B } a) 32 b) 2 c) 4 d) none")]
        public async Task GetAsync_QueryXss(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/api/Question/9339")]
        public async Task GetAsync_Url_Success(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            
            var subject = d["subject"]?.Value<string>();
            var id = d["id"]?.Value<long?>();
            var text = d["text"]?.Value<string>();
            var price = d["price"]?.Value<decimal?>();
            var course = d["course"]?.Value<string>();
            var user = d["user"]?.Value<JObject>();
            var answers = d["answers"]?.Value<JArray>();
            var create = d["create"]?.Value<System.DateTime?>();
            var files = d["files"]?.Value<JArray>();
            var rtl = d["isRtl"]?.Value<bool?>();
            var vote = d["vote"]?.Value<JObject>();

            response.EnsureSuccessStatusCode();
            id.Should().NotBeNull();
            user.Should().NotBeNull();
            rtl.Should().NotBeNull();
            vote.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_Not_Found()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/question/123");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var status = d["status"]?.Value<int?>();

            status.Should().Be(404);
        }
    }
}