using System;

namespace Zbang.Zbox.Domain
{
    public class Student
    {
        protected Student()
        {
            
        }

        public Student(University university, string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            ID = id.PadLeft(9, '0');
            University = university;
        }
        public virtual long StudentId { get; set; }
        public virtual University University { get; set; }

// ReSharper disable once InconsistentNaming - nhibernate
        public virtual string ID { get; set; }
    }
}
