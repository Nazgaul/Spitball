using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cloudents.MobileApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Cloudents.Mobile.Test.IntegrationTests
{
    [TestClass]
    public class CourseApiTests : ServerInit
    {
        [TestMethod]
        public async Task Create_SomeNewCourse_ReturnResult()
        {
            var newCourse = new CreateCourseRequest()
            {
                CourseName = "TestVitali",
                University = 320
            };
            var objectAsJson = JsonConvert.SerializeObject(newCourse);
            var content = new StringContent(objectAsJson, Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("api/course/create", content);
            if (response.IsSuccessStatusCode)
            {
                Assert.IsTrue(true);
                return;
            }

            var body = await response.Content.ReadAsStringAsync();
            Assert.Fail(body);

        }
    }
}