using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class NodeDetails
    {
        public string Name { get; set; }
        public string ParentUrl { get; set; }

        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }

        public LibraryNodeSetting State { get; set; }

        public UserLibraryRelationType UserType { get; set; }
    }
}