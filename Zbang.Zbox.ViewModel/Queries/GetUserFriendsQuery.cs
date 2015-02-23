using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries
{


    public class GetUserFriendsQuery : QueryBase
    {
        public GetUserFriendsQuery(long id)
            : base(id)
        {
        }
    }
}
