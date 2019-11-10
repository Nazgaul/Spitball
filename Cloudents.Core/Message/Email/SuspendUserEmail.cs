using Cloudents.Core.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Message.Email
{
    public class SuspendUserEmail : BaseEmail
    {
        public SuspendUserEmail(string to, CultureInfo info)
            : base(to, "Your account have been suspended", info)
        {

        }
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]

        public override string Campaign => "User Suspended";

        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.System;

        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            {Language.Hebrew , "7c24161f-f447-4760-a35b-5a42e6835766" },
            {Language.English , "b3c762bd-605b-41a2-9c6c-24b01e5adf92" }
        };
    }
}

