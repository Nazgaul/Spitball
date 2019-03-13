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
            var id = leaderboard[0]["id"]?.Value<long?>();
            var name = leaderboard[0]["name"]?.Value<string>();
            var score1 = leaderboard[0]["score"].Value<long>();
            var score2 = leaderboard[1]["score"].Value<long>();
            var uni = leaderboard[0]["university"]?.Value<string>();

            sbl.Should().BeGreaterThan(0);
            leaderboard.Should().HaveCount(10);
            id.Should().BeGreaterThan(0);
            name.Should().NotBeNull();
            score1.Should().BeGreaterThan(score2);
            uni.Should().NotBeNull();
        }

    }
}
