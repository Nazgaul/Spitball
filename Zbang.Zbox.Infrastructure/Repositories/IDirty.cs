using System;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IDirty : ISoftDelete
    {

        bool IsDirty { get; set; }
        Func<bool> ShouldMakeDirty { get; }

        
    }


}