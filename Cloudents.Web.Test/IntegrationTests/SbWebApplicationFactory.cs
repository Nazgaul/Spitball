using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
            //builder.ConfigureAppConfiguration()
        }



    }

    public static class StringExtensions
    {
        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task LogInAsync(this HttpClient client)
        {
            var response = await client.PostAsync("api/LogIn", new StringContent(TestUser.GetTestUser(),
                 Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
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


        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Json serializer")]
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
                    Email = "elad13@cloudents.com",
                    Password = "123456789",
                    FingerPrint = "string"
                };
                return JsonConvert.SerializeObject(user);
            }
        }

    }
}