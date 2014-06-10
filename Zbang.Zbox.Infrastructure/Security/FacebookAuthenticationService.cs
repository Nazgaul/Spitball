using System.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Trace;


namespace Zbang.Zbox.Infrastructure.Security
{
    public class FacebookAuthenticationService : IFacebookService
    {
        const string FacebookPicture = "https://graph.facebook.com/{0}/picture?width={1}&height={1}";


        public async Task<FacebookUserData> FacebookLogIn(string token)
        {
            FacebookUserData user;
            using (HttpClient client = new HttpClient())
            {


                using (var sr = await client.GetStreamAsync("https://graph.facebook.com/me?access_token=" + token))
                {

                    var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUserData));

                    user = dataContractJsonSerializer.ReadObject(sr) as FacebookUserData;

                    if (user == null)
                    {
                        throw new ArgumentNullException("user");
                    }
                    //user can be without email if its not verified in facebook
                    if (string.IsNullOrEmpty(user.email))
                    {
                        throw new ArgumentNullException("email");
                    }
                    user.Image = GetFacebookUserImage(user.id, FacebookPictureType.Square);
                    user.LargeImage = GetFacebookUserImage(user.id, FacebookPictureType.Normal);

                }


            }


            return user;
                {
                    var str = await client.GetStringAsync("https://graph.facebook.com/v1.0/me/friends?limit=50000&access_token=" + token);
                    TraceLog.WriteInfo(string.Format("facebook userid {1} username {2} {0}", str, user.id, user.name));
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On getting user friends from facebook", ex);
            }
            return user;
        }


        public string GetFacebookUserImage(long facebookId, FacebookPictureType type)
        {
            return string.Format(FacebookPicture, facebookId, (int)type);
        }

        public async Task<IEnumerable<FacebookFriendData>> GetFacebookUserFriends(string authToken)
        {
            using (var client = new HttpClient())
            {
                var str = await client.GetStringAsync("https://graph.facebook.com/v1.0/me/friends?fields=picture.height(32).width(32),name&limit=5000&access_token=" + authToken);

                dynamic o = JObject.Parse(str);
                var list = new List<FacebookFriendData>();
                foreach (dynamic friend in o.data)
                {
                    list.Add(new FacebookFriendData
                    {
                        Id = Convert.ToInt64(friend.id),
                        Image = friend.picture.data.url,
                        Name = friend.name
                    });
                }
                var sb = new StringBuilder();
                list.ForEach(s => sb.AppendLine(s.ToString()));
                TraceLog.WriteInfo(sb.ToString());
                return list;
            }
        }

    }
    public enum FacebookPictureType
    {
        Square = 50,
        Normal = 100
    }
}
