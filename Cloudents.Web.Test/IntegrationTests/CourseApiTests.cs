using System;
using System.Collections.Specialized;
using FluentAssertions;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Xunit;
using System.Net;

namespace Cloudents.Web.Test.IntegrationTests
{

    [Collection(SbWebApplicationFactory.WebCollection)]
    public class CourseApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        
        private readonly System.Net.Http.HttpClient _client;
        private readonly object _credentials = new
        {
            email = "blah@cloudents.com",
            password = "123456789",
            fingerPrint = "string"
        };

        private readonly object question = new
        {
            subjectId = "",
            course = "Economics",
            text = "Blah blah blah...",
            price = 1
        };

        private readonly object course = new
        {
            Name = "Economics"
        };

        private readonly object newCourse = new
        {
            name = "NewCourse1"
        };

        private readonly object upload = new
        {
            blobName = "My_Doc.docx",
            name = "My Document",
            type = "Document",
            course = "Economics",
            tags = new { },
            professor = "Mr. Elad",
            price = 0M
        };



        public CourseApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/course/search?term=his")]
        public async Task Get_SomeCourse_ReturnResult(string url)
        {               
            await _client.LogInAsync();
            
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Ask_Course_Without_Uni()
        {   
            await _client.PostAsync("api/LogIn", HttpClient.CreateJsonString(_credentials));

            var response = await _client.PostAsync("api/Question", HttpClient.CreateJsonString(question));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Upload_Doc_Without_Uni()
        {            
            await _client.PostAsync("api/LogIn", HttpClient.CreateJsonString(_credentials));

            var response = await _client.PostAsync("api/Upload", HttpClient.CreateJsonString(upload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Teach_Course()
        {
            await _client.PostAsync("api/LogIn", HttpClient.CreateJsonString(_credentials));

            await _client.PostAsync("api/course/set", HttpClient.CreateJsonString(course));

            var response = await _client.PostAsync("api/course/teach", HttpClient.CreateJsonString(course));

            response.EnsureSuccessStatusCode();
        }

        [Fact(Skip = "this is not a good unit test - need to think about it")]
        public async Task PostAsync_CreateAndDelete_Course()
        {
            UriBuilder uri = new UriBuilder
            {
                Path = "api/course"
            };

            uri.AddQuery(new NameValueCollection
            {
                ["name"] = "NewCourse1"
            });

            await _client.LogInAsync();

            await _client.DeleteAsync(uri.Uri);

            var response = await _client.PostAsync("api/Course/create", HttpClient.CreateJsonString(newCourse));
            response.StatusCode.Should().Be(HttpStatusCode.OK, "Create Course Failed");
            response = await _client.DeleteAsync(uri.Uri);
            response.StatusCode.Should().Be(HttpStatusCode.OK, "Delete Course Failed");
        }

        [Fact(Skip = "this is not a good unit test - need to think about it")]
        public async Task PostAsync_Delete_Course()
        {
            await _client.LogInAsync();
            var uriBuilder = new UriBuilder("api/course");
            uriBuilder.AddQuery(new NameValueCollection()
            {
                ["name"] = "NewCourse1"
            });
            var response = await _client.DeleteAsync(uriBuilder.Uri);

            response.StatusCode.Should().Be(200);
        }

    }
}