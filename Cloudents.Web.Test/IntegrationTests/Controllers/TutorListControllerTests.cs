using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Controllers
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TutorListControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public TutorListControllerTests(SbWebApplicationFactory factory)
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
        public async Task GetAsync_TutorList_OKAsync(string culture)
        {

            var response = await _client.GetAsync($"tutor-list?culture={culture}");
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("en")]
        [InlineData("he")]
        [InlineData("en-IN")]
        public async Task GetAsync_TutorListWithTerm_OKAsync(string culture)
        {

            var response = await _client.GetAsync($"tutor-list/economics?culture={culture}");
            response.EnsureSuccessStatusCode();
        }
    }
}
