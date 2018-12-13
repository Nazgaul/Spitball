
using Cloudents.Search.Interfaces;

namespace Cloudents.FunctionsV2.Binders
{
    public class AzureSearchSyncOutput
    {

        public bool Insert { get; set; }

        public ISearchObject Item { get; set; }

    }
}