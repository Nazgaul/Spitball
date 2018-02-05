using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{

    public class Course
    {
        protected Course()
        {

        }

        public Course(string name, University university)
        {
            RowDetail = new RowDetail();
            Discriminator = CourseType.Academic;
            PrivacySetting = CoursePrivacySetting.AnyoneWithUrl;
            Name = name.Trim();
            University = university;
        }

        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual RowDetail RowDetail  { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual bool IsDirty { get; set; }

        public virtual CourseType Discriminator { get; set; }

        public virtual string CourseCode { get; set; }

        public virtual University University { get; set; }

        public virtual CoursePrivacySetting PrivacySetting { get; set; }
    }
}
