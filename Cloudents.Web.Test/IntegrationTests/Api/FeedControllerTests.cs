using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class FeedControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public FeedControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }



        [Theory]
        [InlineData("api/tutor/search")]
        [InlineData("api/tutor/search?page=1")]
        public async Task GetAsync_Tutor_OK(string url)
        {
            var response = await _client.GetAsync(url);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var result = d["result"]?.Value<JArray>();

            result.Should().NotBeNull();
        }
    }
}
