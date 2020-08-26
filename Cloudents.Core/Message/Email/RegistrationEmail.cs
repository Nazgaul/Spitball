﻿using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Message.Email
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link, CultureInfo info)
            : base(to, "Welcome to Spitball", info)
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }

        public override string Campaign => "Confirm Email";
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.System;

        protected override IDictionary<CultureInfo, string> Templates =>
            new Dictionary<CultureInfo, string>()
        {
            { Language.Hebrew,"85cd5e1c-241c-4a19-9410-7e4c4ca21cbf"},
            {Language.English ,"3668b0b3-dc87-4f93-bf96-cd227a5be004" },
            {Language.EnglishIndia,"1ec4aa02-796d-40bd-8488-54c265871098" }
        };
    }

    public class WelcomeTutorEmail : BaseEmail
    {
        public WelcomeTutorEmail(string to, CultureInfo info, string firstName)
            : base(to, "Welcome to Spitball", info)
        {
            FirstName = firstName;
        }

        public string FirstName { get; private set; }

        public override string? Campaign => "Tutor Email";
        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.System;

        protected override IDictionary<CultureInfo, string>? Templates =>
            new Dictionary<CultureInfo, string>
            {
                {Language.English, "76c0e70d-0e39-4bba-a78d-b5a4e3073bf6"}
            };
    }
}