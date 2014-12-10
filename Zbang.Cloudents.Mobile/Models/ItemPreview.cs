using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ItemPreview
    {
        public ItemPreview(IEnumerable<string> src, int index, bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
            Index = index;
            Src = src;
        }

        public bool IsAuthenticated { get; private set; }
        public int Index { get; private set; }

        public IEnumerable<string> Src { get;private set; }
    }
}