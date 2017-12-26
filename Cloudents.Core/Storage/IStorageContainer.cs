using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Storage
{
    public interface IStorageContainer
    {
        string Name { get; }
        string RelativePath { get; }
    }
}
