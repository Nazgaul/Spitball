using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class ResetPasswordEmail : BaseEmail
    {
        public ResetPasswordEmail(string to, string link)
            : base(to, "forgotPassword", "Password Recovery", campaign:"Password Recovery")
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