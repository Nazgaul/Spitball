using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Query
{
    /// <summary>
    /// Decouples the client of this code from query specific API (e.g. hibernate query api)
    /// </summary>
    public interface IQueryAdapter
    {
        void SetNamedParameter(string name, object value);
    }
}
