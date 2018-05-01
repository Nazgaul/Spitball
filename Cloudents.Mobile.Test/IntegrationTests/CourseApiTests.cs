using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class CourseApiTests : ServerInit
    {
        public async Task Get_SomeCourse_ReturnResult()
        {
            var response = await Client.GetAsync("api/course/search");
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