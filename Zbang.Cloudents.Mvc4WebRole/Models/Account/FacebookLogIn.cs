using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class FacebookLogIn
    {
        public string Token { get; set; }
        public long? UniversityId { get; set; }
        public long? BoxId { get; set; }
    }
}