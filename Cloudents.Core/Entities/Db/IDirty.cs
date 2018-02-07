using System;

namespace Cloudents.Core.Entities.Db
{
    public interface IDirty : ISoftDelete
    {
        bool IsDirty { get; set; }
        Func<bool> ShouldMakeDirty { get; }
    }
}