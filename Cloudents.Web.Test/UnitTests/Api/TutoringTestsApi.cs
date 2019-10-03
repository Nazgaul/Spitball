using Cloudents.Web.Test.IntegrationTests;
using System;
using System.Threading.Tasks;
using Xunit;


namespace Cloudents.Web.Test.UnitTests.Api
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class TutoringTestsApi //: IClassFixture<SbWebApplicationFactory>
    {


        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder
        {
            Path = "api/tutoring/create"
        };


        public TutoringTestsApi(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        //[Fact]
        //public async Task Post_Create_Room()
        //{
        //    var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(""));

        //    response.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task Post_Review()
        //{
        //    var review = new
        //    {
        //        review = "Good one",
        //        rate = 3,
        //        tutor = 160116
        //    };

        //    await _client.LogInAsync();

        //    var response = await _client.PostAsync("api/tutoring/review", HttpClient.CreateJsonString(review));

        //    response.EnsureSuccessStatusCode();
        //}
    }
}
