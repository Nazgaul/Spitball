using System.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class FacebookAuthenticationService : IFacebookService
    {
        private const string FacebookPicture = "https://graph.facebook.com/v2.6/{0}/picture?width={1}&height={1}";

        public async Task<FacebookUserData> FacebookLogOnAsync(string token)
        {
            using (var client = new HttpClient())
            {
                var tData = client.GetStreamAsync("https://graph.facebook.com/v2.4/me?access_token=" + token + "&fields=id,name,first_name,email,middle_name,gender,last_name,locale");
                var tBusiness = client.GetStringAsync("https://graph.facebook.com/v2.4/me/ids_for_business?access_token=" + token);

                await Task.WhenAll(tData, tBusiness).ConfigureAwait(false);

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
                                user.Email = $"{user.Id}@facebook.com";
                            }
                        }
                    }
                }

                var businessObj = JObject.Parse(tBusiness.Result);
                var userId = businessObj["data"].Where(w => (string)w["app"]["name"] == "Cloudents").Select(s => (long?)s["id"]).FirstOrDefault();
                if (userId.HasValue)
                {
                    user.Id = userId.Value;
                }
                user.LargeImage = GetFacebookUserImage(user.Id, FacebookPictureType.Normal);
                return user;
            }
        }

        public string GetFacebookUserImage(long facebookId, FacebookPictureType type)
        {
            return string.Format(FacebookPicture, facebookId, (int)type);
        }

        //public async Task<IEnumerable<FacebookFriendData>> GetFacebookUserFriends(string authenticationToken)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        // we can only use 2100 in an in statement in sql
        //        var str = await client.GetStringAsync("https://graph.facebook.com/v2.2/me/friends?fields=picture.height(32).width(32),name&limit=2100&access_token=" + authenticationToken);

        //        dynamic o = JObject.Parse(str);
        //        var list = new List<FacebookFriendData>();
        //        foreach (dynamic friend in o.data)
        //        {
        //            list.Add(new FacebookFriendData
        //            {
        //                Id = Convert.ToInt64(friend.id),
        //                Image = friend.picture.data.url,
        //                Name = friend.name
        //            });
        //        }
        //        return list;
        //    }
        //}

    }

    public enum FacebookPictureType
    {
        Square = 50,
        Normal = 100
    }
}
