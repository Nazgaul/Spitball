using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            //https://stackoverflow.com/a/7084697/1235448
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(Question)}'");
            Map(x => x.Text).Length(8000).Not.Nullable();
            Map(x => x.Created).Not.Nullable().Not.Update();
            Map(x => x.Updated).Not.Nullable();

            References(x => x.User).Column("UserId")
                .ForeignKey("Question_User").Not.Nullable();
            References(x => x.University).Column("UniversityId").ForeignKey("Question_University").Nullable();
            References(x => x.Course).Column("CourseId").ForeignKey("Question_Course").Nullable();

            References(x => x.Course2).Column("CourseId2").ForeignKey("Question_Course2").Nullable();
            HasMany(x => x.Answers).Access.CamelCaseField(Prefix.Underscore)
                .Inverse()
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan();

            Component(x => x.Status);

        }
    }
}
