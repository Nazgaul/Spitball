using System.Threading.Tasks;
using Cloudents.Api.Models;
using Cloudents.Core.DTOs;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Cloudents.Api.Test.IntegrationTests
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
        public async Task Search_OnlyLocationILEmptyTerm_ReturnResult()
        {
            var response =
                await Client.GetAsync("api/job?location.latitude=31.9200189&location.longitude=34.8016837&term=");
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task Search_OnlyLocationUSEmptyTerm_ReturnResult()
        {
            var response =
                await Client.GetAsync("api/job?location.latitude=40.712&location.longitude=-74.005&term=");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<WebResponseWithFacet<JobDto>>(result);
            obj.Result.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}