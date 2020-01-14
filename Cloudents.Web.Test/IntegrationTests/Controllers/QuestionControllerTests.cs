using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Controllers
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class QuestionControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public QuestionControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }


        [Theory]
        [InlineData("en")]
        [InlineData("he")]
        [InlineData("en-IN")]
        public async Task GetAsync_Document_OKAsync(string culture)
        {

            var response = await _client.GetAsync($"question/7315?culture={culture}");
            response.EnsureSuccessStatusCode();
        }
    }
}
