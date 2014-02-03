using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Zbox.Mvc3WebRole.Factories
{

    public class UserOrFriendData
    {
        public TResult GetData<TResult, TUserQuery, TFriendQuery>(Func<TUserQuery, TResult> UserData,
            Func<TFriendQuery, TResult> FriendData,
            Func<TUserQuery> userQueryGenerator,
            Func<TFriendQuery> friendQueryGenerator,
            string friendUid)
        {
            if (IsFriend(friendUid))
            {
                var friendQuery = friendQueryGenerator();
                return FriendData(friendQuery);

            }
            var userQuery = userQueryGenerator();
            return UserData(userQuery);
        }


        private bool IsFriend(string friendUid)
        {
            return !string.IsNullOrWhiteSpace(friendUid);
        }
    }   
}