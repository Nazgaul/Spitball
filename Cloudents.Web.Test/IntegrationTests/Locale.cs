using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class Locale
    {
        private readonly System.Net.Http.HttpClient _client;

        public Locale(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Theory]
        [InlineData("api/locale")]
        public async Task GetAsync_locale_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();
        }
    }
}
