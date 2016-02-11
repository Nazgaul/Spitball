using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class ClosedNodeDto
    {
        //public ClosedNodeDto()
        //{
        //    User = new List<ClosedNodeUsersDto>();
        //}
        public Guid Id { get; set; }
        public string Name { get; set; }

       // public IList<ClosedNodeUsersDto> User { get; set; }
    }

    public class ClosedNodeUsersDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public UserLibraryRelationType UserType { get; set; }

        public long Id { get; set; }
        public string Url { get; set; }
    }
}
