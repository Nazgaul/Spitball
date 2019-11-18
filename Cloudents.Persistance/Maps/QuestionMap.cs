using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public sealed class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            DynamicUpdate();
            //https://stackoverflow.com/a/7084697/1235448
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(Question)}'");
            Map(x => x.Text).Length(8000).Not.Nullable();
            //Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable().Not.Update();
            Map(x => x.Updated).Not.Nullable();
            Map(x => x.Language).Length(10);
            //Map(x => x.Subject).Nullable().Column("Subject_id").CustomType<QuestionSubject?>();

            References(x => x.User).Column("UserId")
                .ForeignKey("Question_User").Not.Nullable();
            //References(x => x.CorrectAnswer).Column("CorrectAnswer_id").ForeignKey("Question_Answer").Nullable();
            References(x => x.University).Column("UniversityId").ForeignKey("Question_University").Nullable();
            References(x => x.Course).Not.Nullable().Column("CourseId").ForeignKey("Question_Course").Nullable();
            HasMany(x => x.Answers).Access.CamelCaseField(Prefix.Underscore)
                .Inverse()
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan();

            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            //HasMany(x => x.Transactions)
            //    //.Cascade.()
            //    .LazyLoad()
            //    .Inverse();

            //HasMany(x => x.Votes).Access.CamelCaseField(Prefix.Underscore)
            //    .KeyColumns.Add("QuestionId")
            //    .Cascade.AllDeleteOrphan();

            //Map(m => m.VoteCount).Not.Nullable();
            Component(x => x.Status);

            SchemaAction.Validate();
            //DiscriminateSubClassesOnColumn("State");//.Formula($"case when State is Null then 'Ok' else State end");
        }
    }

    public class ItemStateMap : ComponentMap<ItemStatus>
    {
        public ItemStateMap()
        {
            Map(x => x.State)
                .CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            Map(m => m.DeletedOn).Nullable();
            Map(m => m.FlagReason).Nullable();
            References(x => x.FlaggedUser).Column("FlaggedUserId");
        }
    }
}
