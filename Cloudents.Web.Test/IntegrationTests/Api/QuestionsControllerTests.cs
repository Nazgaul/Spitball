using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Cloudents.Infrastructure.Data.Test.IntegrationTests;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Dapper;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class QuestionsControllerTests //:  IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        public QuestionsControllerTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = false
            });
            //_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }


        [Theory]
        [InlineData("api/question/")]
        public async Task GetAsync_Question_Ok(string uri)
        {
            DatabaseFixture _fixture = new DatabaseFixture();

            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'Ok'");
                uri += questionId;
            }

            var response = await _client.GetAsync(uri);

            response.EnsureSuccessStatusCode();

            _fixture.Dispose();
        }

        [Theory]
        [InlineData("api/question/")]
        public async Task GetAsync_Question_NotFound(string uri)
        {
            DatabaseFixture _fixture = new DatabaseFixture();

            using (var conn = _fixture.DapperRepository.OpenConnection())
            {
                var questionId = conn.QueryFirst<long>("select top 1 id from sb.question where state = 'deleted'");
                uri += questionId;
            }

            var response = await _client.GetAsync(uri);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            _fixture.Dispose();
        }
    }


}