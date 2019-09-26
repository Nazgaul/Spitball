using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public sealed class QuestionMap : ClassMapping<Question>
    {
        public QuestionMap()
        {
            DynamicUpdate(true);
            //DynamicUpdate();
            ////https://stackoverflow.com/a/7084697/1235448
            Id(x => x.Id, c => c.Generator(Generators.HighLow, g => g.Params(
                new
                {
                    table = nameof(HiLoGenerator),
                    column = nameof(HiLoGenerator.NextHi),
                    max_lo = 10,
                    where = $"{nameof(HiLoGenerator.TableName)}='{nameof(Question)}'"
                })));
            //Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(Question)}'");
            Property(x => x.Text, c => {
                c.Length(8000);
                c.NotNullable(true);
            });
            //Map(x => x.Text).Length(8000).Not.Nullable();
            Property(x => x.Attachments, c => c.NotNullable(false));
            //Map(x => x.Attachments).Nullable();
            Property(x => x.Created, c => {
                c.NotNullable(true);
                c.Update(false);
            });
            //Map(x => x.Created).Not.Nullable().Not.Update();
            Property(x => x.Updated, c => c.NotNullable(true));
            //Map(x => x.Updated).Not.Nullable();
            Property(x => x.Language, c=> c.Length(10));
            //Map(x => x.Language).Length(10);
            ////Map(x => x.Subject).Nullable().Column("Subject_id").CustomType<QuestionSubject?>();

            ManyToOne(x => x.User, c => {
                c.ForeignKey("Question_User");
                c.NotNullable(true);
                c.Column(cl =>
                {
                    cl.SqlType("bigint");
                    cl.Name("UserId");
                });
            });
            //References(x => x.User).Column("UserId")
            //    .ForeignKey("Question_User").Not.Nullable();
            ManyToOne(x => x.CorrectAnswer, c => {
                c.Column("CorrectAnswer_id");
                c.ForeignKey("Question_Answer");
                c.NotNullable(false);
            });
            //References(x => x.CorrectAnswer).Column("CorrectAnswer_id").ForeignKey("Question_Answer").Nullable();
            ManyToOne(x => x.University, c => {
                c.Column("UniversityId");
                c.ForeignKey("Question_University");
                c.NotNullable(false);
            });
            //References(x => x.University).Column("UniversityId").ForeignKey("Question_University").Nullable();
            ManyToOne(x => x.Course, c => {
                c.NotNullable(true);
                c.Column("CourseId");
                c.ForeignKey("Question_Course");
            });
            //References(x => x.Course).Not.Nullable().Column("CourseId").ForeignKey("Question_Course").Nullable();

            Bag(x => x.Answers, c => {
                c.Inverse(true);
                c.Lazy(CollectionLazy.Extra);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Key(k => {
                    k.Column("QuestionId");
                });
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
            }, a => a.OneToMany());
            //HasMany(x => x.Answers).Access.CamelCaseField(Prefix.Underscore)
            //    .Inverse()
            //    .ExtraLazyLoad()
            //    .Cascade.AllDeleteOrphan();

            Bag(x => x.Transactions, c => {
                c.Inverse(true);
                c.Lazy(CollectionLazy.Lazy);
                c.Key(k => k.Column("QuestionId"));
            }, a => a.OneToMany());
            ////DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            //HasMany(x => x.Transactions)
            //    //.Cascade.()
            //    .LazyLoad()
            //    .Inverse();

            Bag(x => x.Votes, c => {
                c.Key(k => k.Column("QuestionId"));
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
            }, a => a.OneToMany());
            //HasMany(x => x.Votes).Access.CamelCaseField(Prefix.Underscore)
            //    .KeyColumns.Add("QuestionId")
            //    .Cascade.AllDeleteOrphan();

            Property(m => m.VoteCount, c => c.NotNullable(true));
            //Map(m => m.VoteCount).Not.Nullable();
            Component(x => x.Status);
            //Component(x => x.Status);
            //SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
            ////DiscriminateSubClassesOnColumn("State");//.Formula($"case when State is Null then 'Ok' else State end");
        }
    }

    public class ItemStateMap : ComponentMapping<ItemStatus>
    {
        public ItemStateMap()
        {
            Property(x => x.State, c => {
                c.Type<GenericEnumStringType<ItemState>>();
                c.NotNullable(true);
            });
            //Map(x => x.State)
            //    .CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            Property(m => m.DeletedOn, c => c.NotNullable(false));
            //Map(m => m.DeletedOn).Nullable();
            Property(m => m.FlagReason, c => c.NotNullable(false));
            //Map(m => m.FlagReason).Nullable();
            ManyToOne(x => x.FlaggedUser, c => c.Column("FlaggedUserId"));
            //References(x => x.FlaggedUser).Column("FlaggedUserId");
        }
    }
}
