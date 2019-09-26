using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{

    [CollectionDefinition(SbWebApplicationFactory.WebCollection)]
    public class WebCollection : ICollectionFixture<SbWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class SbWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public const string WebCollection = "WebCollection";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //builder.UseEnvironment(Startup.IntegrationTestEnvironmentName);
        }



    }

    public static class HttpClient
    {
        public static async Task LogInAsync(this System.Net.Http.HttpClient client)
        {
            await client.PostAsync("api/LogIn", new StringContent(TestUser.GetTestUser(),
                 Encoding.UTF8, "application/json"));
        }

        public static StringContent CreateString(string str)
        {
            return new StringContent(str, Encoding.UTF8, "application/json");
        }

        public static StringContent CreateJsonString(object obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            return new StringContent(str, Encoding.UTF8, "application/json");
        }

        //public string User => TestUser.GetTestUser();


        private class TestUser
        {
            private TestUser()
            {

            }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FingerPrint { get; set; }

            public static string GetTestUser()
            {
                var user = new TestUser
                {
                    Email = "hadarkeidar90@gmail.com",
                    Password = "Cloudents69",
                    FingerPrint = "string"
                };
                return JsonConvert.SerializeObject(user);
            }
        }
               
    }
}