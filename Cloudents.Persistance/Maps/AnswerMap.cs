using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Text).Length(Answer.MaxLength);
            //Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Language).Length(10);

            References(x => x.User).Column("UserId").ForeignKey("Answer_User").Not.Nullable();
            References(x => x.Question).Column("QuestionId").ForeignKey("Answer_Question").Not.Nullable();

            Component(x => x.Status);
            //References(x => x.FlaggedUser).Column("FlaggedUserId").ForeignKey("AnswerFlagged_User");
            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            //HasMany(x => x.Transactions)
            //    //.Cascade()
            //    .LazyLoad()
            //    .Inverse();

            //HasMany(x => x.Votes)
            //    .Access.CamelCaseField(Prefix.Underscore)
            //    .KeyColumns.Add("AnswerId")
            //    .Inverse().Cascade.AllDeleteOrphan();
            //Map(x => x.VoteCount);
            SchemaAction.Validate();
            //DiscriminateSubClassesOnColumn("State");
        }

        
    }
}