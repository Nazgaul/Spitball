using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum FeedType : int
    {
        None = 0,

        [EnumDescription(typeof(QuestionResource), "NewCourse")]
        CreatedCourse = 1,
        [EnumDescription(typeof(QuestionResource), "AddedFiles")]
        AddedItems = 2,
        [EnumDescription(typeof(QuestionResource), "NewGroup")]
        CreatedBox = 3
    }
}