using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    using System;

    public partial class Course
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDirty { get; set; }

        public BoxType Discriminator { get; set; }

        public string CourseCode { get; set; }

        public virtual University University { get; set; }

    }
}
