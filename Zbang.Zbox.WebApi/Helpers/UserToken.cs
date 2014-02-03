using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.WebApi.Helpers
{
    [Serializable]
    public class UserToken
    {
        public long UserId { get; set; }
        public DateTime ExpireTokenTime { get; set; }
    }
}