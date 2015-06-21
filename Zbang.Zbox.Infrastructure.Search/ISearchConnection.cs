using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Search;
using RedDog.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface ISearchConnection
    {
        IndexQueryClient IndexQuery { get; }
        IndexManagementClient IndexManagement { get; }
        SearchServiceClient SearchClient { get; }
        bool IsDevelop { get; }
    }
}
