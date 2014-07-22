using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeBoxesDto
    {
        public NodeBoxesDto(IEnumerable<NodeDto> nodes, int nodesCount, IEnumerable<BoxDto> boxes, int boxesCount, NodeDto parent)
        {
            Nodes = new PagedDto<NodeDto>(nodes, nodesCount);
            Boxes = new PagedDto<BoxDto>(boxes, boxesCount);
            Parent = parent;
        }

        public PagedDto<NodeDto> Nodes { get; set; }
        public PagedDto<BoxDto> Boxes { get; set; }

        public NodeDto Parent { get; set; }
    }
}
