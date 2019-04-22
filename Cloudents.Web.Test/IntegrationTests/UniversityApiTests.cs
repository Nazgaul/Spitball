using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UniversityApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UniversityApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAsync_SomeLocation_Ok()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("api/university?Location.Longitude=-74.005&Location.Latitude=40.712");
            var result = await response.Content.ReadAsStringAsync();
            var d = JObject.Parse(result);
            var p = d["universities"].Values();
            p.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Post_Set_Uni()
        {
            var client = _factory.CreateClient();

            string uni = "{\"id\":\"bdb71a15-62ed-4fab-8a76-a98200e81a53\"}";

            await client.LogInAsync();

            var response = await client.PostAsync("api/university/set", new StringContent(uni, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/api/university")]
        public async Task GetAsync_OK(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

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
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var uni = d["universities"].Value<JArray>();

            var id = uni[0]["id"]?.Value<string>();

            uni.Should().NotBeNull();
        }

        [Fact]
        public async Task PostAsync_Create_Success()
        {
            var client = _factory.CreateClient();
            await client.LogInAsync();
            var response = await client.PostAsync("api/University/create", 
                new StringContent("{\"name\":\"Open University\",\"country\":\"IL\"}", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Create_Failure()
        {
            var client = _factory.CreateClient();


            await client.LogInAsync();

            var response = await client.PostAsync("api/University/create", 
                new StringContent("{\"name\":\"Open Uni\",\"country\":\"IL\"}",
                Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}