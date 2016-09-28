using System;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Invite
    {
        public Guid InviteId { get; set; }


        public const string CookieName = "invId";
    }


    public class UniversityCookie
    {
        public long UniversityId { get; set; }

        public string UniversityName { get; set; }


        public const string CookieName = "uni1";
    }
}