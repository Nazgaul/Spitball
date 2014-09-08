using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;

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
            Url = UrlConsts.BuildDepartmentUrl(id, name, university.UniversityName);
            NoOfBoxes = 0;
        }

        public virtual long Id { get; private set; }
               
        public virtual University University { get; private set; }
        public virtual University University2 { get; private set; }
        public virtual string Name { get; private set; }
               
        public  string Url { get; private set; }
        public virtual  int NoOfBoxes { get; private set; }

        public virtual void UpdateNumberOfBoxes(int count)
        {
            NoOfBoxes = count;
        }
    }
}
