using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class LeaderboardApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public LeaderboardApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAsync_OK()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/homepage/leaderboard");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var sbl = d["SBL"]?.Value<int?>();
            var leaderboard = d["leaderBoard"]?.Value<JArray>();
            sbl.Should().BeGreaterThan(0);
            leaderboard.Should().HaveCount(10);

            for(int i = 0; i < 10; i++)
            {
                var id = leaderboard[i]["id"]?.Value<long?>();
                id.Should().BeGreaterThan(0);
                var name = leaderboard[i]["name"]?.Value<string>();
                name.Should().NotBeNull();
                var score1 = leaderboard[i]["score"].Value<long>();
                score1.Should().BeGreaterThan(0);
                var uni = leaderboard[i]["university"]?.Value<string>();
                uni.Should().NotBeNull();
            }
        }

    }
}
