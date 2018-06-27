using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link) : base(to, "register","Welcome to Spitball")
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }
    }
}