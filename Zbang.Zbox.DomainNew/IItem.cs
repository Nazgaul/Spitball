using System;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public interface IItem 
    {
        long Id { get; }
        bool IsReviewed { get; set; }
        Language Language { get; set; }
        Func<bool> ShouldMakeDirty { get; set; }
        //ISet<ItemTag> ItemTags { get; }

        void AddTag(Tag tag, TagType type);
        void RemoveTag(string tag);

        //CourseTag CourseTag { get; set; }
    }
}