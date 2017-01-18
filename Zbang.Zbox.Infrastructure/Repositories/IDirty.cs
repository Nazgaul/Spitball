using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IDirty : ISoftDelete
    {

        DirtyState IsDirty { get; set; }
        Func<bool> ShouldMakeDirty { get; }

        
    }


}