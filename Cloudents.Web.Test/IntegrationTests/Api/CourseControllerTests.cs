using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class CourseControllerTests //: IClassFixture<SbWebApplicationFactory>
    {

        private readonly System.Net.Http.HttpClient _client;

        private readonly object _credentials = new
        {
            email = "blah@cloudents.com",
            password = "123456789",
            fingerPrint = "string"
        };


        private readonly object _course = new
        {
            Name = "Economics"
        };

        public CourseControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/course/search?term=his")]
        public async Task Get_SomeCourse_ReturnResultAsync(string url)
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }


        [Theory]
        [InlineData("api/course/")]
        [InlineData("api/course/1")]
        public async Task CourseApiTestGet_LogIn_OkAsync(string api)
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
        public async Task GetAsync_Get_CoursesAsync()
        {
          //  await _client.LogInAsync();

//            _uri.Path = "api/course/search";

            var response = await _client.GetAsync("api/course/search");

            response.Should().NotBeNull();

            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }


     

        [Theory]
        [InlineData("api/course/search")]
        public async Task GetAsync_Course_OKAsync(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

    }
}