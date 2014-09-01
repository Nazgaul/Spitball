using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeBoxesDto
    {
        public NodeBoxesDto(IEnumerable<NodeDto> nodes, IEnumerable<BoxDto> boxes)
        {
            Nodes = nodes;
            Boxes = boxes;
        }

        public IEnumerable<NodeDto> Nodes { get; set; }
        public IEnumerable<BoxDto> Boxes { get; set; }

    }
}
