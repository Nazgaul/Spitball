﻿using System.Linq;
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


        public async Task<FacebookUserData2> FacebookLogIn(string token)
        {
            FacebookUserData2 user;
            using (var client = new HttpClient())
            {
                using (var sr = await client.GetStreamAsync("https://graph.facebook.com/v2.2/me?access_token=" + token))
                {
                    
                    var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUserData2));

                    user = dataContractJsonSerializer.ReadObject(sr) as FacebookUserData2;

                    if (user == null)
                    {
                        throw new NullReferenceException("user");
                    }
                    //user can be without email if its not verified in facebook
                    if (string.IsNullOrEmpty(user.email))
                    {
                        user.email = string.Format("{0}@facebook.com", user.id);
                    }
                    user.Image = GetFacebookUserImage(user.id, FacebookPictureType.Square);
                    user.LargeImage = GetFacebookUserImage(user.id, FacebookPictureType.Normal);

                    //if (user.education != null)
                    //{
                    //    TraceLog.WriteInfo("facebook user education: " +
                    //                       string.Join(" ", user.education.Select(s => s.school)));
                    //}
                }
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
