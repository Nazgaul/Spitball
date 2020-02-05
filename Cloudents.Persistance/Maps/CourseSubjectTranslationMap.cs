using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class CourseSubjectTranslationMap : ClassMap<CourseSubjectTranslation>
    {
        public CourseSubjectTranslationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(r => r.Subject).Column("SubjectId").ForeignKey("FK_subject_translation").Not.Nullable();
            References(r => r.Language).Column("LanguageId").ForeignKey("FK_language_translation").Not.Nullable();
            Map(m => m.NameTranslation).Length(300).Not.Nullable();
            Table("SubjectTranslation");
        }
    }
}
