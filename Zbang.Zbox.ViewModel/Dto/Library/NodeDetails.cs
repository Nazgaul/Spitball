using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeDetails
    {
        public string Name { get; set; }
        public string ParentUrl { get; set; }

        public LibraryNodeSettings State { get; set; }
    }
}