using System.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;


namespace Zbang.Zbox.Infrastructure.Security
{
    public class FacebookAuthenticationService : IFacebookService
    {
        const string FacebookPicture = "https://graph.facebook.com/{0}/picture?width={1}&height={1}";


        public async Task<FacebookUserData> FacebookLogIn(string token)
        {
            using (var client = new HttpClient())
            {
                
                var tData = client.GetStreamAsync("https://graph.facebook.com/v2.4/me?access_token=" + token + "&fields=id,name,first_name,email,middle_name,gender,last_name,locale");
                var tBusiness = client.GetStringAsync("https://graph.facebook.com/v2.4/me/ids_for_business?access_token=" + token);

                await Task.WhenAll(tData, tBusiness);

                FacebookUserData user;
                using (var s = tData.Result)
                {
                    using (var sr = new StreamReader(s))
                    {
                        using (var reader = new JsonTextReader(sr))
                        {
                            var serializer = new JsonSerializer();
                            user = serializer.Deserialize<FacebookUserData>(reader);
                            if (user == null)
                            {
                                throw new NullReferenceException("user");
                            }
                            if (string.IsNullOrEmpty(user.Email))
                            {
                                user.Email = string.Format("{0}@facebook.com", user.Id);
                            }

                        }
                    }
                }

                var businessObj = JObject.Parse(tBusiness.Result);
                var userId = businessObj["data"].Where(w => ((string)w["app"]["name"] == "Cloudents")).Select(s => (long?)s["id"]).FirstOrDefault();
                if (userId.HasValue)
                {
                    user.Id = userId.Value;
                }
                //user.Image = GetFacebookUserImage(user.Id, FacebookPictureType.Square);
                user.LargeImage = GetFacebookUserImage(user.Id, FacebookPictureType.Normal);
                return user;



                //using (var sr = await client.GetStreamAsync("https://graph.facebook.com/v2.4/me?access_token=" + token + "&fields=id,name,first_name,email,middle_name,gender,last_name"))
                //{

                //    var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUserData2));

                //    user = dataContractJsonSerializer.ReadObject(sr) as FacebookUserData2;

                //    if (user == null)
                //    {
                //        throw new NullReferenceException("user");
                //    }
                //    //user can be without email if its not verified in facebook
                //    if (string.IsNullOrEmpty(user.email))
                //    {
                //        user.email = string.Format("{0}@facebook.com", user.id);
                //    }
                //    user.Image = GetFacebookUserImage(user.id, FacebookPictureType.Square);
                //    user.LargeImage = GetFacebookUserImage(user.id, FacebookPictureType.Normal);

                //    //if (user.education != null)
                //    //{
                //    //    TraceLog.WriteInfo("facebook user education: " +
                //    //                       string.Join(" ", user.education.Select(s => s.school)));
                //    //}
                //}
            }



        }


        public string GetFacebookUserImage(long facebookId, FacebookPictureType type)
        {
            return string.Format(FacebookPicture, facebookId, (int)type);
        }

        public async Task<IEnumerable<FacebookFriendData>> GetFacebookUserFriends(string authToken)
        {
            using (var client = new HttpClient())
            {
                // we can only use 2100 in an in statement in sql
                var str = await client.GetStringAsync("https://graph.facebook.com/v2.2/me/friends?fields=picture.height(32).width(32),name&limit=2100&access_token=" + authToken);

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
