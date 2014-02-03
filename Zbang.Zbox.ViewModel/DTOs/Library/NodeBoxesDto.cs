using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Library
{
    public class NodeBoxesDto
    {
        public NodeBoxesDto(IEnumerable<NodeDto> nodes, int nodesCount, IEnumerable<BoxDto> boxes, int boxesCount, NodeDto parent)
        {
            Nodes = new PagedDto2<NodeDto>(nodes, nodesCount);
            Boxes = new PagedDto2<BoxDto>(boxes, boxesCount);
            Parent = parent;
        }

        public PagedDto2<NodeDto> Nodes { get; set; }
        public PagedDto2<BoxDto> Boxes { get; set; }

        public NodeDto Parent { get; set; }
    }
}
