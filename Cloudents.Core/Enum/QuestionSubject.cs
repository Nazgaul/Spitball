using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum.Resources;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Enum
{
    public enum QuestionSubject
    {
        [ResourceDescription(typeof(QuestionSubjectResources), "Mathematics")]
        Mathematics = 1,
        [ResourceDescription(typeof(QuestionSubjectResources), "History")]
        History = 2,
        [ResourceDescription(typeof(QuestionSubjectResources), "Literature")]
        Literature = 3,
        [ResourceDescription(typeof(QuestionSubjectResources), "Biology")]
        Biology = 4,
        [ResourceDescription(typeof(QuestionSubjectResources), "Chemistry")]
        Chemistry = 5,
        [ResourceDescription(typeof(QuestionSubjectResources), "Physics")]
        Physics = 6,
        [ResourceDescription(typeof(QuestionSubjectResources), "Economics")]
        Economics = 7,
        [ResourceDescription(typeof(QuestionSubjectResources), "Social Studies")]
        SocialStudies = 8,
        [ResourceDescription(typeof(QuestionSubjectResources), "Geography")]
        Geography = 9,
        [ResourceDescription(typeof(QuestionSubjectResources), "Health & Medicine")]
        HealthMedicine = 10,
        [ResourceDescription(typeof(QuestionSubjectResources), "Arts")]
        Arts = 11,
        [ResourceDescription(typeof(QuestionSubjectResources), "Business")]
        Business = 12,
        [ResourceDescription(typeof(QuestionSubjectResources), "Computer Science")]
        ComputerScience = 13,
        [ResourceDescription(typeof(QuestionSubjectResources), "Languages")]
        Languages = 14,
        [ResourceDescription(typeof(QuestionSubjectResources), "Psychology")]
        Psychology = 15,
        [ResourceDescription(typeof(QuestionSubjectResources), "Miscellaneous"), OrderValue(1)]
        Miscellaneous = 16,
        [ResourceDescription(typeof(QuestionSubjectResources), "Religion/Philosophy")]
        ReligionPhilosophy = 17,
        [ResourceDescription(typeof(QuestionSubjectResources), "Education")]
        Education = 18,
        [ResourceDescription(typeof(QuestionSubjectResources), "Technology")]
        Technology = 19,
        [ResourceDescription(typeof(QuestionSubjectResources), "Law & Politics")]
        LawPolitics = 20,
        [ResourceDescription(typeof(QuestionSubjectResources), "BlockChain")]
        BlockChain = 21,
        [ResourceDescription(typeof(QuestionSubjectResources), "Accounting")]
        Accounting = 22,
        [ResourceDescription(typeof(QuestionSubjectResources), "Sports")]
        Sports = 23,
        [ResourceDescription(typeof(QuestionSubjectResources), "Anthropology")]
        Anthropology


        
    }

    public static class QuestionSubjectMethod
    {
        public static IEnumerable<QuestionSubject> GetValues()
        {
            var values = EnumExtension.GetValues<QuestionSubject>();
            return GetValues(values);
        }
        public static IEnumerable<QuestionSubject> GetValues(IEnumerable<QuestionSubject> values)
        {
            //var values = EnumExtension.GetValues<QuestionSubject>();
            return values.OrderBy(o => o.GetAttributeValue<OrderValueAttribute>()?.Order ?? 0)
                .ThenBy(o => o.GetEnumLocalization());
        }
    }
}