using FluentAssertions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class CourseApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        
        private readonly SbWebApplicationFactory _factory;

        public CourseApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("api/course/search?term=his")]
        public async Task Get_SomeCourse_ReturnResult(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Ask_Course_Without_Uni()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string question = "{\"subjectId\":\"\",\"course\":\"Economics\",\"text\":\"Blah blah blah...\",\"price\":10,\"files\":[]}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Question", new StringContent(question, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Upload_Doc_Without_Uni()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string upload = "{\"blobName\": \"My_Doc.docx\",\"name\":\"My Document\",\"type\":\"Document\",\"course\":\"Economics\",\"tags\":[\"string\"],\"professor\":\"Mr. Elad\",\"price\":0}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Upload", new StringContent(upload, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Teach_Course()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string course = "{\"Name\":\"Economics\"}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            await client.PostAsync("api/course/set", new StringContent(course, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/course/teach", new StringContent(course, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_CreateAndDelete_Course()
        {
            var client = _factory.CreateClient();

            await client.LogInAsync();

            await client.DeleteAsync("api/course?name=\"NewCourse1\"");
            var response = await client.PostAsync("api/Course/create", new StringContent("{\"name\":\"NewCourse1\"}", Encoding.UTF8, "application/json"));
            response.StatusCode.Should().Be(200, "Create Course Failed");
            response = await client.DeleteAsync("api/course?name=\"NewCourse1\"");
            response.StatusCode.Should().Be(200, "Delete Course Failed");
        }

        //[Fact]
        //public async Task PostAsync_Delete_Course()
        //{
        //    var client = _factory.CreateClient();

        //    string cred = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

        //    CancellationToken token;

        //    await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

        //    var response = await client.DeleteAsync("api/course?name=\"NewCourse1\"", token);

        //    response.StatusCode.Should().Be(200);
        //}

    }
}