using Cloudents.Web.Test.IntegrationTests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.UnitTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TabsControllerTests //: IClassFixture<SbWebApplicationFactory>
    {

        private readonly System.Net.Http.HttpClient _client;

        private readonly UriBuilder _uri = new UriBuilder()
        {
            Path = "/note"
        };


        public TabsControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }



        [Fact(Skip = "Obsolete")]
        public async Task GetAsync_StudyDoc()
        {
            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();

            response.RequestMessage.RequestUri.Should().Be(_uri.Uri);
        }

        [Fact(Skip = "Obsolete")]
        public async Task GetAsync_HW()
        {
            _uri.Path = "/ask";

            var response = await _client.GetAsync(_uri.Path);

            response.EnsureSuccessStatusCode();

            response.RequestMessage.RequestUri.Should().Be(_uri.Uri);
        }
    }
}
