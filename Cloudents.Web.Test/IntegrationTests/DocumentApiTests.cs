using Xunit;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class DocumentApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;
        private readonly object cred = new
        {
            email = "elad@cloudents.com",
            password = "123456789",
            fingerPrint = "string"
        };
        private readonly object doc1 = new
        {
            mime_type = "",
            name = "C:\\Users\apolo\\Downloads\\ACloudFan.pdf",
            phase = "start",
            size = 1027962
        };
        private readonly object doc2 = new
        {
            mime_type = "",
            name = "C:\\Users\apolo\\Downloads\\Capture 2.png",
            phase = "start",
            size = 1027962
        };
        private readonly object doc3 = new
        {
            mime_type = "",
            name = "C:\\Users\apolo\\Downloads\\ספיטבול.docx",
            phase = "start",
            size = 1027962
        };
        private readonly object doc4 = new
        {
            mime_type = "",
            name = "C:\\Users\apolo\\Downloads\\doc4",
            phase = "start",
            size = 1027962
        };


        public DocumentApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("api/document")]
        [InlineData("/api/document?page=1")]
        public async Task GetAsync_OK(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            
            var result = d["result"]?.Value<JArray>();
            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();
            var next = d["nextPageLink"]?.Value<string>();
            
            var id = result[0]["id"]?.Value<long?>();
            var university = result[0]["university"]?.Value<string>();
            var course = result[0]["course"]?.Value<string>();
            var title = result[0]["title"]?.Value<string>();
            var user = result[0]["user"]?.Value<JObject>();
            var views = result[0]["views"]?.Value<int?>();
            var downloads = result[0]["downloads"]?.Value<int?>();
            var docUrl = result[0]["url"]?.Value<string>();
            //var source = result[0]["source"]?.Value<string>();
            var dateTime = result[0]["dateTime"]?.Value<DateTime?>();
            var vote = result[0]["vote"]?.Value<JObject>();
            var price = result[0]["price"]?.Value<double?>();

            result.Should().NotBeNull();
            filters.Should().NotBeNull();
            type.Should().HaveCountGreaterThan(3);

            id.Should().NotBeNull();
            university.Should().NotBeNull();
            course.Should().NotBeNull();
            title.Should().NotBeNull();
            user.Should().NotBeNull();
            views.Should().NotBeNull();
            downloads.Should().NotBeNull();
            docUrl.Should().NotBeNull();
            //source.Should().NotBeNull();
            dateTime.Should().NotBeNull();
            vote.Should().NotBeNull();
            price.Should().BeGreaterOrEqualTo(0);

            //if (url == "/api/document?page=1")
                //next.Should().Be("http://localhost:80/api/Document?Page=2");
        }

        [Fact]
        public async Task GetAsync_Filters()
        {
            var response = await _client.GetAsync("/api/document");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var filters = d["filters"]?.Value<JArray>();
            var type = filters[0]["data"]?.Value<JArray>();

            filters.Should().NotBeNull();
            type.Should().HaveCountGreaterThan(3);
        }

        [Fact]
        public async Task PostAsync_Upload_Regular_FileName()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/Document/upload", HttpClient.CreateJsonString(doc1));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_FileName_With_Space()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/Document/upload", HttpClient.CreateJsonString(doc2));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Hebrew_FileName()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/Document/upload", HttpClient.CreateJsonString(doc3));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Without_File_Extension()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync("api/Document/upload", HttpClient.CreateJsonString(doc4));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var str = await response.Content.ReadAsStringAsync();
        }
    }
}
