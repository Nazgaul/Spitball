using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
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
            IsDeleted = false;
        }

        public virtual long Id { get; set; }

        public virtual string Name { get; protected set; }

        public virtual RowDetail RowDetail  { get; protected set; }

        public virtual bool IsDeleted { get; protected set; }


       // public virtual bool IsDirty { get; set; }

        public virtual CourseType Discriminator { get; protected set; }

        public virtual string CourseCode { get; set; }

        public virtual University University { get; protected set; }

        public virtual CoursePrivacySetting PrivacySetting { get; protected set; }
    }
}
