using System;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public interface IItem 
    {
        long Id { get; }
        Language Language { get; set; }
        Func<bool> ShouldMakeDirty { get; set; }
        //ISet<ItemTag> ItemTags { get; }
    }
}