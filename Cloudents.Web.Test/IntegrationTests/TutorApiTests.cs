using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TutorApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        //private UriBuilder _uri = new UriBuilder()
        //{
        //    Path = "api/tutor"
        //};


        public TutorApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAsync_ReturnResult_OK(bool logIn)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync("api/tutor");
            response.EnsureSuccessStatusCode();
        }


        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAsyncWithCourse_ReturnResult_OK(bool logIn)
        {
            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync("api/tutor?course=Economics");
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("gfc",false)]
        [InlineData("Economics", false)]
        [InlineData("Math", false)]
        [InlineData("", false)]
        [InlineData("gfc", true)]
        [InlineData("Economics", true)]
        [InlineData("Math", true)]
        [InlineData("", true)]
        public async Task GetAsync_Search_Without_Results(string term, bool logIn)
        {

            if (logIn)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync($"api/tutor/search?term={term}");
            response.EnsureSuccessStatusCode();
            //var str = await response.Content.ReadAsStringAsync();

            //str.Should().BeEmpty();
        }

        //[Fact]
        //public async Task Get_OK_Result()
        //{
        //    var response = await _client.GetAsync(_uri.Path);

        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //}

        //[Fact(Skip = "We did a hole markup change to tutor")]
        //public async Task Post_Create_Room()
        //{
        //    var response = await _client.PostAsync("api/tutoring/create", null);

        //    var str = await response.Content.ReadAsStringAsync();

        //    var d = JObject.Parse(str);

        //    var result = d["name"]?.Value<string>();

        //    response.StatusCode.Should().Be(HttpStatusCode.OK);

        //    result.Should().NotBeNull();
        //}

        //[Fact]
        //public async Task Get_NonExist_Tutor()
        //{
        //    await _client.LogInAsync();

        //    var response = await _client.GetAsync(_uri.Path + "/search?term=fsdfds");

        //    var str = await response.Content.ReadAsStringAsync();

        //    var d = JObject.Parse(str);

        //    var result = d["result"]?.Value<JArray>();

        //    result.Should().BeEmpty();
        //}
    }
}