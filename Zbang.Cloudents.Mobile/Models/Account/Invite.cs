using System;

namespace Zbang.Cloudents.Mobile.Models.Account
{
    public class Invite
    {
        public Guid InviteId { get; set; }


        public const string CookieName = "invId";
    }
}