using System;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using Dapper;
using FluentAssertions;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests.Api
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
            DatabaseFixture fixture = new DatabaseFixture();

            await _client.LogInAsync();

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            using (var conn = fixture.DapperRepository.OpenConnection())
            {
                var studyRoomId = conn.QueryFirst<Guid>("select top 1 id from sb.studyroom where tutorid = 159489");
                uri += studyRoomId.ToString();
            }

            response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            fixture.Dispose();
        }

        [Theory]
        [InlineData("api/studyRoom")]
        public async Task GetAsync_StudyRoom_Unauthorized(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("api/studyRoom")]
        public async Task GetUserLobbyStudyRooms_StudyRoom_Ok(string uri)
        {
            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
