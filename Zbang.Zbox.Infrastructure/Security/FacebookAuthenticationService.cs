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

            using (HttpClient client = new HttpClient())
            {

                FacebookUserData user;
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
                    return user;
                }
            }
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
