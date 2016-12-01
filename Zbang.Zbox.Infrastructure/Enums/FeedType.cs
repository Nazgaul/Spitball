using Zbang.Zbox.Infrastructure.Enums.Resources;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Enums
{
    public enum FeedType : int
    {
        None = 0,

        [ResourceDescription(typeof(QuestionResource), "NewCourse")]
        CreatedCourse = 1,
        [ResourceDescription(typeof(QuestionResource), "AddedFiles")]
        AddedItems = 2,
        [ResourceDescription(typeof(QuestionResource), "NewGroup")]
        CreatedBox = 3
    }
}