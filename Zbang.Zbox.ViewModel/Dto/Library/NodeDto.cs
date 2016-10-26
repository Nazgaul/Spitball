using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Url { get; set; }

        public int? NoBoxes { get; set; }

        public int? NoDepartment { get; set; }

        public LibraryNodeSetting? State { get; set; }

        public UserLibraryRelationType? UserType { get; set; }
    }

    public class SmallNodeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
