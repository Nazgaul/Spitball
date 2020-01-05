using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class DocumentControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly object _credentials = new
        {
            email = "blah@cloudents.com",
            password = "123456789",
            fingerPrint = "string"
        };

        private readonly object _upload = new
        {
            blobName = "My_Doc.docx",
            name = "My Document",
            type = "Document",
            course = "Economics",
            tags = new { },
            professor = "Mr. Elad",
            price = 0M
        };

        private readonly object _doc1 = new
        {
            mime_type = "",
            name = "ACloudFan.pdf",
            phase = "start",
            size = 1027962
        };
        private readonly object _doc2 = new
        {
            mime_type = "",
            name = "Capture 2.png",
            phase = "start",
            size = 1027962
        };
        private readonly object _doc3 = new
        {
            mime_type = "",
            name = "ספיטבול.docx",
            phase = "start",
            size = 1027962
        };
        private readonly object _doc4 = new
        {
            mime_type = "",
            name = "Doc4",
            phase = "start",
            size = 1027962
        };

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/document"
        };



        public DocumentControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("api/document/similar", "Economics", 50413L, false)]
        [InlineData("api/document/similar", "Economics", 50413L, true)]
        public async Task GetSimilarDocuments_OkAsync(string url, string course, long documentId, bool authUser)
        {
            var endPoint = $"{url}?course={course}&documentId={documentId}";
            if (authUser)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync(endPoint);
            response.StatusCode.Should().Be(200);

            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }

        [Fact]
        public async Task PostAsync_Upload_Regular_FileNameAsync()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClientExtensions.CreateJsonString(_doc1));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_FileName_With_SpaceAsync()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClientExtensions.CreateJsonString(_doc2));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Hebrew_FileNameAsync()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClientExtensions.CreateJsonString(_doc3));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Without_File_ExtensionAsync()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClientExtensions.CreateJsonString(_doc4));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("document/המסלול-האקדמי-המכללה-למנהל")]
        public async Task ShortUrl_Invalid_404Async(string url)
        {
            var response = await _client.GetAsync(url);

            var p = response.Headers.Location;
            p.Should().Be("/Error/NotFound");
            //Assert.EndsWith("error/notfound", p.AbsolutePath);
        }

        [Theory]
        [InlineData("api/document/2999")]
        public async Task Valid_Url_200Async(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();

            str.IsValidJson().Should().BeTrue("the invalid string is {0}", str);
        }

        [Fact]
        public async Task Upload_Doc_Without_UniAsync()
        {
            await _client.PostAsync(_uri.Path, HttpClientExtensions.CreateJsonString(_credentials));

            _uri.Path = "api/upload";

            var response = await _client.PostAsync(_uri.Path, HttpClientExtensions.CreateJsonString(_upload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
