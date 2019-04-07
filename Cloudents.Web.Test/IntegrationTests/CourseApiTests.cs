﻿using FluentAssertions;
using System.Net.Http;
using System.Text;
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

            string crad = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\"}";

            await client.PostAsync("api/LogIn", new StringContent(crad, Encoding.UTF8, "application/json"));

            // Act
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Ask_Course_Without_Uni()
        {
            var client = _factory.CreateClient();

            string crad = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string question = "{\"subjectId\":\"mathematics\",\"text\":\"Blah blah blah...\",\"price\":2,\"files\":[\"string\"],\"course\":\"Economics\"}";

            await client.PostAsync("api/LogIn", new StringContent(crad, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Question", new StringContent(question, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Upload_Doc_Without_Uni()
        {
            var client = _factory.CreateClient();

            string crad = "{\"email\":\"blah@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            string upload = "{\"blobName\": \"My_Doc.docx\",\"name\":\"My Document\",\"type\":\"Document\",\"course\":\"Economics\",\"tags\":[\"string\"],\"professor\":\"Mr. Elad\",\"price\":0}";

            await client.PostAsync("api/LogIn", new StringContent(crad, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Upload", new StringContent(upload, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
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