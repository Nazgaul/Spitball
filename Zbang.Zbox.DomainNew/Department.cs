using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Department
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public string Year { get; private set; }

        public University University { get; private set; }
    }
}
