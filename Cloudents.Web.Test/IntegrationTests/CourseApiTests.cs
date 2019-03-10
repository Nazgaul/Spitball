using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class CourseApiTests : IClassFixture<SbWebApplicationFactory>
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

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        //TODO: not sure why this fails.
        //[TestMethod]
        //public async Task Create_SomeNewCourse_ReturnResult()
        //{
        //    var newCourse = new CreateCourseRequest()
        //    {
        //        CourseName = "TestVitali",
        //        University = 320
        //    };
        //    var objectAsJson = JsonConvert.SerializeObject(newCourse);
        //    var content = new StringContent(objectAsJson, Encoding.UTF8,
        //        "application/json");
        //    var response = await Client.PostAsync("api/course/create", content);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Assert.IsTrue(true);
        //        return;
        //    }

        //    var body = await response.Content.ReadAsStringAsync();
        //    Assert.Fail(body);

        //}
    }
}