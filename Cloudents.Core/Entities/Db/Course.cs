using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    using System;

    public partial class Course
    {
        protected Course()
        {

        }

        public Course(string name, long universityId)
        {
            RowDetail = new RowDetail();
            Discriminator = CourseType.Academic;
            PrivacySetting = CoursePrivacySetting.AnyoneWithUrl;
            //IsDeleted = false;
            Name = name.Trim();
            UniversityId = universityId;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public RowDetail RowDetail  { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDirty { get; set; }

        public CourseType Discriminator { get; set; }

        public string CourseCode { get; set; }

        public virtual University University { get; set; }
        public long? UniversityId { get; set; }

        public CoursePrivacySetting PrivacySetting { get; set; }

    }
}
