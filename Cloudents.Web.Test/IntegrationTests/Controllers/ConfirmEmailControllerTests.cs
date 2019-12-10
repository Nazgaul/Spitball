using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Controllers
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class ConfirmEmailControllerTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;


        public ConfirmEmailControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/ConfirmEmail?Id=1013 and 3064%3d3064&code=CfDJ8GwX8MRfevpGmOMc9ZOhCGPS2rnhH%2BTS9R5O9FC2UXZ5Xnw%2BbHKrDwKlyKoIAY5ni78hQFwu3hTro16UrzKLGnsK53D9IWY8xB%2FgJgnRL%2FJuetfe9VIMwFtg1IKBk9TKGCDRhu75WomXbmCKnwTYGDFjosNloE4%2FfBnzotk1mhdcmrsv8LQV1Kh0bO47wuENz4FQGcFdzc43FZ%2FhlEszgKU%3D")]
        public async Task Get_WrongLongId_500Page(string url)
        {
            var response = await _client.GetAsync(url);
            var p = response.Headers.Location;
            p.OriginalString.Should().Be("/");
        }
    }
}