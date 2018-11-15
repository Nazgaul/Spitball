﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class ConfirmEmailControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ConfirmEmailControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/ConfirmEmail?Id=1013 and 3064%3d3064&code=CfDJ8GwX8MRfevpGmOMc9ZOhCGPS2rnhH%2BTS9R5O9FC2UXZ5Xnw%2BbHKrDwKlyKoIAY5ni78hQFwu3hTro16UrzKLGnsK53D9IWY8xB%2FgJgnRL%2FJuetfe9VIMwFtg1IKBk9TKGCDRhu75WomXbmCKnwTYGDFjosNloE4%2FfBnzotk1mhdcmrsv8LQV1Kh0bO47wuENz4FQGcFdzc43FZ%2FhlEszgKU%3D")]
        public async Task Get_WrongLongId_500Page(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);
            var p = response.Headers.Location;
            Assert.True(p.OriginalString == "/");
        }
    }
}