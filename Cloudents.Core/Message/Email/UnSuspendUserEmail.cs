using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Application.Message.Email
{
    class UnSuspendUserEmail : BaseEmail
    {
        public UnSuspendUserEmail(string to, CultureInfo info)
            : base(to, "Your account have been unsuspended", info)
        {

        }
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]

        public override string Campaign => "User UnSuspended";

        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            {Language.Hebrew.Culture , "bc982dec-f632-447f-889c-7eb67ea37e21" },
            {Language.English.Culture , "d7a2ebcb-3670-473f-b745-43c6e769348c" }
        };
    }
}
