using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Dashboard
{
    public class WallDto 
    {
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public long UserId { get; set; }
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string Action { get; set; }


        public string UniName { get; set; }

        public string Url { get; set; }

        public string UserUrl { get; set; }
    }
}
