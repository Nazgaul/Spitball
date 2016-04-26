using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum FeedType : int
    {
        None = 0,

        [EnumDescriptionAttribute(typeof(QuestionResource), "NewCourse")]
        CreatedCourse = 1,
        [EnumDescriptionAttribute(typeof(QuestionResource), "AddedFiles")]
        AddedItems = 2,
        [EnumDescriptionAttribute(typeof(QuestionResource), "NewGroup")]
        CreatedBox = 3
    }
}