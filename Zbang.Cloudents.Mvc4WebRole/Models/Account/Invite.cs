using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Invite
    {
        public Guid InviteId { get; set; }


        public const string CookieName = "invId";
    }
}