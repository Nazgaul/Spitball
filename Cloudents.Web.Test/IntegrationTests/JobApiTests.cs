using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web.Test.IntegrationTests
{
    [TestClass]
    public class JobApiTests : ServerInit
    {
        [TestMethod]
        public async Task Search_SomeQuery_ReturnResult()
        {
            var response = await Client.GetAsync("/api/Job?Term=Android&Location.Point.Longitude=40.0&Location.Point.Latitude=-51.&Highlight=true&Page=0").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Search_SomeQuery2_ReturnResult()
        {
            var response = await Client.GetAsync("/api/Job?Location.Point.Latitude=33.71&Location.Point.Longitude=-117.9478&Facet=Part+Time%2CInternship&Page=1").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Search_OnlyLocationILEmptyTerm_ReturnResult()
        {
            var response =
                await Client.GetAsync("api/job?term=");
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Search_OnlyLocationUSEmptyTerm_ReturnResult()
        {
            var response =
                await Client.GetAsync("api/job?location.Point.latitude=40.712&location.Point.longitude=-74.005&term=");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var d = JObject.Parse(result);
            var p = d["result"]["result"].Values();
            
            p.Should().HaveCountGreaterOrEqualTo(1);
        }

        
    }
}