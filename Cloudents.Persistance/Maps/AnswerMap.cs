using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;

namespace Cloudents.Persistance.Maps
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class AnswerMap : ItemMap<Answer>
    {
        public AnswerMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Text).Length(8000);
            Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Language).Length(10);

            References(x => x.User).Column("UserId").ForeignKey("Answer_User").Not.Nullable();
            References(x => x.Question).Column("QuestionId").ForeignKey("Answer_Question").Not.Nullable();

            References(x => x.FlaggedUser).Column("FlaggedUserId").ForeignKey("AnswerFlagged_User");
            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            HasMany(x => x.Transactions)
                //.Cascade()
                .LazyLoad()
               
                .Inverse();
            HasMany(x => x.Votes).KeyColumns.Add("AnswerId")
                .Inverse().Cascade.AllDeleteOrphan();

            SchemaAction.None();
            //DiscriminateSubClassesOnColumn("State");
        }

        
    }

    //public class AnswerDeletedMap : SubclassMap<AnswerDeleted>
    //{
    //    public AnswerDeletedMap()
    //    {

    //        DiscriminatorValue(ItemState.Deleted);
    //    }
    //}

    //public class AnswerPendingMap : SubclassMap<AnswerPending>
    //{
    //    public AnswerPendingMap()
    //    {

    //        DiscriminatorValue(ItemState.Pending);
    //    }
    //}

    //public class AnswerApprovedMap : SubclassMap<AnswerApproved>
    //{
    //    public AnswerApprovedMap()
    //    {
    //        DiscriminatorValue(ItemState.Ok);
    //    }
    //}
}