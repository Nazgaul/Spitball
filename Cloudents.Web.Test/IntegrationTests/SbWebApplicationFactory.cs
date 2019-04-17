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

        public string User => TestUser.GetTestUser();


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
                    Email = "elad@cloudents.com",
                    Password = "123456789",
                    FingerPrint = "string"
                };
                return JsonConvert.SerializeObject(user);
            }
        }

    }
}