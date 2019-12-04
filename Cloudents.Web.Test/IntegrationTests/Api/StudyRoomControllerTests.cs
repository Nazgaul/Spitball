using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using Dapper;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class StudyRoomApiTests
    {
        private readonly System.Net.Http.HttpClient _client;

        public StudyRoomApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("api/studyRoom/")]
        public async Task GetAsync_StudyRoom_Ok(string uri)
        {
            DatabaseFixture _fixture = new DatabaseFixture();

            await _client.LogInAsync();

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var studyRoomId = conn.QueryFirst<Guid>("select top 1 id from sb.studyroom where tutorid = 159489");
                uri += studyRoomId.ToString();
            }

            response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            _fixture.Dispose();
        }

        [Theory]
        [InlineData("api/studyRoom")]
        public async Task GetAsync_StudyRoom_Unauthorized(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
