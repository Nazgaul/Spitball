using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Department
    {
        protected Department()
        {
            
        }
        public Department(long id, string name, University university)
        {
            Id = id;
            Name = name;
            University = university;
        }

        public long Id { get; private set; }

        public University University { get; private set; }
        public University University2 { get; private set; }
        public string Name { get; private set; }
    }
}
