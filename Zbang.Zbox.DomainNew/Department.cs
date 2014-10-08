using System;
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
            if (university == null) throw new ArgumentNullException("university");
            Id = id;
            Name = name;
            University = university;
            Url = UrlConsts.BuildDepartmentUrl(id, name, university.UniversityName);
            NoOfBoxes = 0;
        }

        public virtual long Id { get; private set; }

        public virtual University University { get; protected set; }
        public virtual University University2 { get; protected set; }
        public virtual string Name { get; private set; }
               
        public  string Url { get; private set; }
        public virtual  int NoOfBoxes { get; private set; }

        public virtual void UpdateNumberOfBoxes(int count)
        {
            if (count < 0)
            {
                count = 0;
            }
            NoOfBoxes = count;
        }
    }
}
