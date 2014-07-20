using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
   public  class StoreCategory
    {
       public virtual int Id { get; set; }
       public virtual int ParentId { get; set; }
       public virtual string Name { get; set; }
       public virtual int Order { get; set; }
    }
}
