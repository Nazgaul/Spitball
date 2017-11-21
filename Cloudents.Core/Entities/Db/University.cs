using System;
using System.Collections.Generic;

namespace Cloudents.Core.Entities.Db
{
    

    public partial class University
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public University()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Courses = new HashSet<Course>();
        }

        public long Id { get; set; }

        public string Image { get; set; }

        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Country { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsDirty { get; set; }

        public int? UtcOffset { get; set; }

        public float? Latitude { get; set; }

        public float? Longitude { get; set; }

        public string Extra { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }


    }
}
