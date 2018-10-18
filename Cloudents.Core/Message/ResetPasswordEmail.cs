using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class ResetPasswordEmail : BaseEmail
    {
        public ResetPasswordEmail(string to, string link, CultureInfo info)
            : base(to,  "Password Recovery", info)
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

        public override string Campaign => "Password Recovery";
        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            { Language.Hebrew.Culture,"e4334fe9-b71d-466f-80ea-737bf16d9c81"},
            {Language.English.Culture ,"4f915763-1f9e-4b81-9ed7-09c1201c677b" }
        };
    }
}