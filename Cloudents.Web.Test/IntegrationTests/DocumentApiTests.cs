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



        public DocumentApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        [Theory]
        [InlineData("api/feed",false)]
        [InlineData("/api/feed?page=1",false)]
        [InlineData("api/feed", true)]
        [InlineData("/api/feed?page=1", true)]
        public async Task GetAsync_OK(string url, bool authUser)
        {
            if (authUser)
            {
                await _client.LogInAsync();
            }
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);
            
            var result = d["result"]?.Value<JArray>();

            var next = d["nextPageLink"]?.Value<string>();

            result.Should().NotBeNull();

            if (url == _uri.Path + "?page=1")
                next.Should().Be(_uri.Path + "?page=2");
        }

       

        [Fact]
        public async Task PostAsync_Upload_Regular_FileName()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc1));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_FileName_With_Space()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc2));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Hebrew_FileName()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc3));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Upload_Without_File_Extension()
        {
            await _client.LogInAsync();

            var response = await _client.PostAsync(_uri.Path + "/upload", HttpClient.CreateJsonString(_doc4));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_OldDocument_OK()
        {
            _uri.Path = "document/Box%20Read%20for%20hotmail%20user/Load%20Stress%20Testing%20Multimi2.docx/457";

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("document/המסלול-האקדמי-המכללה-למנהל")]
        public async Task ShortUrl_Invalid_404(string url)
        {
            var response = await _client.GetAsync(url);

            var p = response.Headers.Location;
            p.Should().Be("/Error/NotFound");
            //Assert.EndsWith("error/notfound", p.AbsolutePath);
        }

        [Theory]
        [InlineData("api/document/2999")]
        public async Task Valid_Url_200(string url)
        {
            var response = await _client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Upload_Doc_Without_Uni()
        {
            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_credentials));

            _uri.Path = "api/upload";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_upload));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
