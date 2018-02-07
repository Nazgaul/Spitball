namespace Cloudents.Core.Entities.Db
{
    public class University
    {
        protected University()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            //Courses = new HashSet<Course>();
            //RowDetail = new RowDetail();
        }

        public virtual long Id { get; set; }

        //public string Image { get; set; }

       // public string Name { get; set; }

        //public RowDetail RowDetail { get; set; }

        //public string Country { get; set; }
        public virtual bool IsDeleted { get; set; }

        //public bool IsDirty { get; set; }

        //public int? UtcOffset { get; set; }

        //public float? Latitude { get; set; }

        //public float? Longitude { get; set; }

        //public string Extra { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Course> Courses { get; set; }
    }
}
