﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Department
    {
        public virtual long Id { get; private set; }
        public virtual string Name { get; private set; }
        public virtual string Year { get; private set; }
                
        public virtual University University { get; private set; }
    }
}
