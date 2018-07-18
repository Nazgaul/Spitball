using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Cloudents.Api.Test.IntegrationTests
{
    [TestClass]
    public class UniversityApiTests : ServerInit
    {
        [TestMethod]
        public async Task GetAsync_SomeLocation_Ok()
        {
            var response = await Client.GetAsync("api/university?Location.Longitude=-74.005&Location.Latitude=40.712");
            var result = await response.Content.ReadAsStringAsync();
            var d = JObject.Parse(result);
            var p = d["universities"].Values();
            p.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}