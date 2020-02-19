using Cloudents.Core.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Message.Email
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

        public override UnsubscribeGroup UnsubscribeGroup => UnsubscribeGroup.Update;

        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            {Language.Hebrew , "8e4035cd-c7d4-408a-a4ed-e0fa68f4e444" },
            {Language.English , "43ec6c46-0478-45f8-ac13-a83c83a4076d" },
            {Language.EnglishIndia,"06872a4a-578d-4f37-b777-c26c508e42c3" }

        };
    }

}
