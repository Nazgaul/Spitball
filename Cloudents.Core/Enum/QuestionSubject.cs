using System.Collections.Generic;
using System.Linq;
using Cloudents.Application.Attributes;
using Cloudents.Application.Extension;
using Cloudents.Common.Enum;

namespace Cloudents.Application.Enum
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