using System.Collections.Generic;
using System.Linq;
using Cloudents.Common;
using Cloudents.Core.Attributes;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Enum
{
    

    public static class QuestionSubjectMethod
    {
        public static IEnumerable<QuestionSubject> GetValues()
        {
            var values = EnumExtension.GetValues<QuestionSubject>();
            return GetValues(values);
        }
        public static IEnumerable<QuestionSubject> GetValues(IEnumerable<QuestionSubject> values)
        {
            return values.OrderBy(o => o.GetAttributeValue<OrderValueAttribute>()?.Order ?? 0)
                .ThenBy(o => o.GetEnumLocalization());
        }
    }
}