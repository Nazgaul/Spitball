using System;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link) : base(to, "register","Welcome to Spitball")
        {
            Link = link;
        }

        public string Link { get; private set; }

    }
}