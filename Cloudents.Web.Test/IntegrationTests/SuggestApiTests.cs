using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SuggestApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public SuggestApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
    }
}