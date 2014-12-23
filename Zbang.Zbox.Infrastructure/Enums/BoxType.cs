
using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
// ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    public enum BoxType : int
    {
        Box = 0,
        Academic = 2
    }

    // ReSharper disable once EnumUnderlyingTypeIsInt - need for dapper
    public enum FeedType : int
    {
        None = 0,

        [EnumDescription(typeof(QuestionResource), "NewCourse")]
        CreatedCourse = 1,
        [EnumDescription(typeof(QuestionResource), "AddedFiles")]
        AddedItems = 2
    }
}
