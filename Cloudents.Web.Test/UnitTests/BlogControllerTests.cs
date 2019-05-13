using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class BlogControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        private readonly System.Net.Http.HttpClient _client;

        private readonly UriBuilder _uri = new UriBuilder()
        {
            Path = "blog"
        };

        private readonly string _mediumLink = "https://medium.com/@spitballstudy";


        public BlogControllerTests(SbWebApplicationFactory factory)
        {
            //_factory = factory;
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Redirect_Test()
        {
            var response = await _client.GetAsync(_uri.Path);

            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            response.Headers.Location.Should().Be(_mediumLink);
        }
    }
}
