using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UrlControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        // private readonly SbWebApplicationFactory _factory;
        private readonly System.Net.Http.HttpClient _client;


        public UrlControllerTests(SbWebApplicationFactory factory)
        {
            // _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            _client.DefaultRequestHeaders.Referrer = new Uri("https://www.spitball.co/note");
        }

        [Fact(Skip = "Not relevant anymore")]
        public async Task GetAsync_NoQueryString_RedirectHomePage()
        {
            var response = await _client.GetAsync("/url");

            var p = response.Headers.Location;

            Assert.True(p.OriginalString == "/");
        }


        [Fact(Skip = "Not relevant anymore")]
        public async Task GetAsync_CramHost_RedirectToCram()
        {
            var url = "https%3A%2F%2Fwww.cram.com%2Fflashcards%2Faccounting-2491586";

            var response = await _client.GetAsync($"/url?url={url}&host=Cram&location=3");

            var p = response.Headers.Location;

            var decode = System.Net.WebUtility.UrlDecode(url);

            p.Should().Be(decode);
        }

        [Fact(Skip = "Not relevant anymore")]
        public async Task GetAsync_SomeGibrishUrl_HomePage()
        {
            var url =
                "/url?url=https:%2F%2Fwww.google.com&host=google' and 3481%3d3481'-- &location=23";

            var response = await _client.GetAsync(url);

            var p = response.Headers.Location;

            Assert.True(p.OriginalString == "/");
        }

        [Fact(Skip = "Not relevant anymore")]
        //[ExpectedException(typeof(ArgumentException))]
        public async Task GetAsync_NoWhiteList_500Page()
        {
            var url =
                "/url?url=https:%2F%2Fwww.google.com&host=google&location=23";

            //await Client.GetAsync(url);

            await Assert.ThrowsAsync<ArgumentException>(() => _client.GetAsync(url));
        }

        [Fact(Skip = "Not relevant anymore")]
        public async Task GetAsync_SomeGibrishUrl2_HomePage()
        {
            var url = "/url?url=https:%2f%2fwww.google.com%00fasvp\"><a>q2ifd&host=google&location=23";

            var response = await _client.GetAsync(url);

            var p = response.Headers.Location;

            Assert.True(p.OriginalString == "/");
        }

        [Fact(Skip = "Not relevant anymore")]
        public async Task GetAsync_NotValidUrl_HomePage()
        {
            var url = "url?url=%2fetc%2fpasswd&host=google&location=23";

            var response = await _client.GetAsync(url);

            var p = response.Headers.Location;

            Assert.True(p.OriginalString == "/");
        }
    }
}