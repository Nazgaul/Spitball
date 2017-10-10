using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudents.Core.Request
{
    public interface IQuery
    {
        string CacheKey { get; }
    }
}
