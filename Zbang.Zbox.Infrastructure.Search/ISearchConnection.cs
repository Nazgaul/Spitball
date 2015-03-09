using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedDog.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface ISearchConnection
    {
        IndexQueryClient IndexQuery { get; }
        IndexManagementClient IndexManagement { get; }

        bool IsDevelop { get; }
    }
}
