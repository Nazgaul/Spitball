using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Trace;


namespace Zbang.Zbox.Infrastructure.Security
{
    public class FacebookAuthenticationService : IFacebookAuthenticationService
    {
        const string facebookPicture = "https://graph.facebook.com/{0}/picture?width={1}&height={1}";


        public async Task<FacebookUserData> FacebookLogIn(string token)
        {
            FacebookUserData user;
            using (HttpClient client = new HttpClient())
            {

                
                using (var sr = await client.GetStreamAsync("https://graph.facebook.com/me?access_token=" + token))
                {

                    var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUserData));
                    user = dataContractJsonSerializer.ReadObject(sr) as FacebookUserData;

                    //user can be without email if its not verified in facebook
                    if (string.IsNullOrEmpty(user.email))
                    {
                        throw new ArgumentNullException("email");
                    }
                    user.Image = GetFacebookUserImage(user.id, FacebookPictureType.square);
                    user.LargeImage = GetFacebookUserImage(user.id, FacebookPictureType.normal);
                    
                }
            }

            using (var client = new HttpClient())
            {
                var str = await client.GetStringAsync("https://graph.facebook.com/v2.0/me/friends?limit=50000&access_token=" + token);
                TraceLog.WriteInfo(str);
            }
            return user;
        }


        public string GetFacebookUserImage(long facebookId, FacebookPictureType type)
        {
            return string.Format(facebookPicture, facebookId, (int)type);
        }

    }
    public enum FacebookPictureType
    {
        square = 50,
        normal = 100
    }
}
