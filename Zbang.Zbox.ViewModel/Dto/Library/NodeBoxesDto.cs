using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeBoxesDto
    {
        public IEnumerable<NodeDto> Nodes { get; set; }
        public IEnumerable<BoxDto> Boxes { get; set; }

        public NodeDetails Details { get; set; }

    }
}
