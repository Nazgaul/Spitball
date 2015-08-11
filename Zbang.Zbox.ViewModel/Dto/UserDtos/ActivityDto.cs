using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class ActivityDto
    {
        public long BoxId { get; set; }
        public string Content { get; set; }
        public Guid Id { get; set; }
        public string Type { get; set; }

        public string BoxName { get; set; }
    }
}
