﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Message.Email
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
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.System;

        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            { Language.Hebrew,"e4334fe9-b71d-466f-80ea-737bf16d9c81"},
            {Language.English ,"4c4fcea0-3c7c-403b-8593-67c580b3133b" }
        };
    }
}