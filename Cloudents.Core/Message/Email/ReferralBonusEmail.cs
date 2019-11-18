using Cloudents.Core.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Message.Email
{
    public class ReferralBonusEmail : BaseEmail
    {
        public ReferralBonusEmail(string to, CultureInfo info)
            : base(to, "One of your friends just joined Spitball", info)
        {

        }
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]

        public override string Campaign => "Referral User";

        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.Update;

        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {

            {Language.English ,"5560b10d-bb82-43bf-85eb-1f2947bdb2d8" }
        };
    }
}
