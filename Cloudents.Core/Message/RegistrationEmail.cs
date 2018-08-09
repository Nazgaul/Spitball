using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link) 
            : base(to, "register","Welcome to Spitball", "SendGrid", "Email", "Registration")
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }
    }
}