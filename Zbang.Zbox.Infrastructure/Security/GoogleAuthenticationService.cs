using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class GoogleAuthenticationService : IGoogleService
    {
       public async Task<GoogleUserData> GoogleLogOnAsync(string token)
       {
           using (var client = new HttpClient())
           {
               using (
                   var googleStream =
                       await client.GetStreamAsync("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + token).ConfigureAwait(false))
               {
                   using (var sr = new StreamReader(googleStream))
                   {
                       using (var reader = new JsonTextReader(sr))
                       {
                           var serializer = new JsonSerializer();
                           var user = serializer.Deserialize<GoogleUserData>(reader);
                           if (user == null)
                           {
                               throw new NullReferenceException("user");
                           }
                           return user;
                       }
                   }
               }
           }
       }
    }
}
