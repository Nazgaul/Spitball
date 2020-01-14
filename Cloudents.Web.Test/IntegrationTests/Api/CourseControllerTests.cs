﻿using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
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

        private readonly object _newCourse = new
        {
            name = "NewCourse1"
        };

        //private readonly object _upload = new
        //{
        //    blobName = "My_Doc.docx",
        //    name = "My Document",
        //    type = "Document",
        //    course = "Economics",
        //    tags = new { },
        //    professor = "Mr. Elad",
        //    price = 0M
        //};

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/login"
        };



        public CourseControllerTests(SbWebApplicationFactory factory)
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

            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }

      

        [Fact]
        public async Task Teach_Course()
        {
            await _client.PostAsync(_uri.Path, HttpClientExtensions.CreateJsonString(_credentials));

            _uri.Path = "api/course/set";

            await _client.PostAsync(_uri.Path, HttpClientExtensions.CreateJsonString(_course));

            _uri.Path = "api/course/teach";

            var response = await _client.PostAsync(_uri.Path, HttpClientExtensions.CreateJsonString(_course));

            response.EnsureSuccessStatusCode();
        }

        [Fact(Skip = "this is not a good unit test - need to think about it")]
        public async Task PostAsync_CreateAndDelete_Course()
        {
            _uri.Path = "api/course";

            _uri.AddQuery(new NameValueCollection
            {
                ["name"] = "NewCourse1"
            });

            await _client.LogInAsync();

            await _client.DeleteAsync(_uri.Uri);

            var response = await _client.PostAsync(_uri.Path + "/create", HttpClientExtensions.CreateJsonString(_newCourse));

            response.StatusCode.Should().Be(HttpStatusCode.OK, "Create Course Failed");

            response = await _client.DeleteAsync(_uri.Uri);

            response.StatusCode.Should().Be(HttpStatusCode.OK, "Delete Course Failed");
        }

        [Fact(Skip = "this is not a good unit test - need to think about it")]
        public async Task PostAsync_Delete_Course()
        {
            await _client.LogInAsync();

            _uri.Path = "api/course";

            _uri.AddQuery(new NameValueCollection()
            {
                ["name"] = "NewCourse1"
            });

            var response = await _client.DeleteAsync(_uri.Uri);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Get_Courses()
        {
            await _client.LogInAsync();

            _uri.Path = "api/course/search";

            var response = await _client.GetAsync(_uri.Path);

            response.Should().NotBeNull();

            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }

    }
}