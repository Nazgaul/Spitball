namespace Zbang.Zbox.Domain
{
    public class Student
    {
        public virtual long StudentId { get; set; }
        public virtual University University { get; set; }

// ReSharper disable once InconsistentNaming - nhibernate
        public virtual string ID { get; set; }
    }
}
