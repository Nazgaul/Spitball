using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class LogInControllerTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public LogInControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        

       

        [Fact]
        public async Task Post_Login_With_EmailAsync()
        {
            await _client.LogInAsync();
        }
    }
}
