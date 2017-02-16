using System;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public interface IItem 
    {
        long Id { get; }
        Language Language { get; set; }
        Func<bool> ShouldMakeDirty { get; set; }
        //ISet<ItemTag> ItemTags { get; }

        void AddTag(Tag tag, TagType type);

        //CourseTag CourseTag { get; set; }
    }
}