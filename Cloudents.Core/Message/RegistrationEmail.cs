using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link) 
            : base(to, "register","Welcome to Spitball", "SendGrid", "Email", "Confirm Email")
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }
    }

    [Serializable]
    public class ResetPasswordEmail : BaseEmail
    {
        public ResetPasswordEmail(string to, string link)
            : base(to, null, "Reset Your Email", "SendGrid", "Email", "Reset Email")
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }

        public override string ToString()
        {
            return $"Paste this link in browser {Link}";
        }
    }
}