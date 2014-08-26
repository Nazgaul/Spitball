using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeBoxesDto
    {
        public NodeBoxesDto(IEnumerable<NodeDto> nodes, IEnumerable<BoxDto> boxes, NodeDto parent)
        {
            Nodes = nodes;
            Boxes = boxes;
            Parent = parent;
        }

        public IEnumerable<NodeDto> Nodes { get; set; }
        public IEnumerable<BoxDto> Boxes { get; set; }

        public NodeDto Parent { get; set; }
    }
}
