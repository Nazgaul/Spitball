using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Application.Message.Email
{
    public class QuestionDeletedEmail : BaseEmail
    {
        public QuestionDeletedEmail(string to, CultureInfo info)
            : base(to, "Your question was deleted", info)
        {

        }
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]

        public override string Campaign => "Question Deleted";

        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            {Language.Hebrew.Culture , "8e4035cd-c7d4-408a-a4ed-e0fa68f4e444" },
            {Language.English.Culture , "43ec6c46-0478-45f8-ac13-a83c83a4076d" }
        };
    }
   
}
