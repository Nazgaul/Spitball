using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Security
{
   public  class FacebookFriendData
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Image { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0} Name: {1} Image {2}", Id, Name, Image);
        }
    }
}
