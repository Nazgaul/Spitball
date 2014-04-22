using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Student
    {
        public virtual long StudentId { get; set; }
        public virtual University University { get; set; }

        public virtual string ID { get; set; }
    }
}
