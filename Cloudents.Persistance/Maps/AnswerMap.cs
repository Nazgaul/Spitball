using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class AnswerMap : ClassMapping<Answer>
    {
        public AnswerMap()
        {
            DynamicUpdate(true);
            Id(x => x.Id, c => c.Generator(Generators.GuidComb)); //.GeneratedBy.GuidComb();
            Property(x => x.Text, c => c.Length(Answer.MaxLength)); //Map(x => x.Text).Length(Answer.MaxLength);
            Property(x => x.Attachments, c => c.NotNullable(false)); //Map(x => x.Attachments).Nullable();
            Property(x => x.Created, c => c.NotNullable(true)); //Map(x => x.Created).Not.Nullable();
            Property(x => x.Language, c => c.Length(10)); //Map(x => x.Language).Length(10);
            ManyToOne(x => x.User, m =>
            {
                m.Column("UserId");
                m.ForeignKey("Answer_User");
                m.NotNullable(true);
            }); //References(x => x.User).Column("UserId").ForeignKey("Answer_User").Not.Nullable();

            ManyToOne(x => x.Question, m =>
            {
                m.Column("QuestionId");
                m.ForeignKey("Answer_Question");
                m.NotNullable(true);
            });
            //References(x => x.Question).Column("QuestionId").ForeignKey("Answer_Question").Not.Nullable();

            Component(x => x.Status); //Component(x => x.Status);
            ////References(x => x.FlaggedUser).Column("FlaggedUserId").ForeignKey("AnswerFlagged_User");
            ////DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            Bag(x => x.Transactions, m => {
                m.Inverse(true);
                m.Lazy(CollectionLazy.Lazy);
                m.Key(k => k.Column("AnswerId"));
            }, a => a.OneToMany());
            //HasMany(x => x.Transactions)
            //    //.Cascade()
            //    .LazyLoad()
            //    .Inverse();

            Bag<Vote>("_votes", m => {
                m.Inverse(true);
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Key(k => k.Column("AnswerId"));
            }, n => n.OneToMany(o => { }));
            //HasMany(x => x.Votes)
            //    .Access.CamelCaseField(Prefix.Underscore)
            //    .KeyColumns.Add("AnswerId")
            //    .Inverse().Cascade.AllDeleteOrphan();

            Property(x => x.VoteCount);
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //Map(x => x.VoteCount);
            //SchemaAction.Validate();
            ////DiscriminateSubClassesOnColumn("State");
        }


    }
}