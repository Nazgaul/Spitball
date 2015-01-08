using System;

namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IDirty : ISoftDeletable
    {
        bool IsDirty { get; set; }
        Func<bool> ShouldMakeDirty { get; }

        //IEnumerable<IDirty> ItemsToDirty { get; }

        
    }
}