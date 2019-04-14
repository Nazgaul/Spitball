using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class UserApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UserApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task Get_User()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            var response = await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            response = await client.GetAsync("api/course/search?term=fsdfds");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            courses.Should().BeEmpty();
        }
    }
}
