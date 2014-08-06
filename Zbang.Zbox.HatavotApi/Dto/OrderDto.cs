using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Store.Dto
{
   public  class OrderDto
    {
       public OrderDto(long orderId, string productName)
       {
           OrderId = orderId;
           ProductName = productName;
       }

       public long OrderId { get; private set; }
       public string ProductName { get; set; }
    }
}
