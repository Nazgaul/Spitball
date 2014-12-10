using System;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Invite
    {
        public Guid InviteId { get; set; }


        public const string CookieName = "invId";
    }
}