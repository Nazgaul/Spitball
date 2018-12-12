using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]

    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Text).Length(8000);
            Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable();
            //Map(x => x.State).Not.Nullable();
            Component(x => x.Item, t =>
            {
                QuestionMap.ItemComponentPartialMapping(t);
                t.HasMany(x => x.Votes).KeyColumns.Add("AnswerId")
                    .Inverse().Cascade.AllDeleteOrphan();
            });

            References(x => x.User).Column("UserId").ForeignKey("Answer_User").Not.Nullable();
            References(x => x.Question).Column("QuestionId").ForeignKey("Answer_Question").Not.Nullable();

            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            HasMany(x => x.Transactions)
                //.Cascade()
                .LazyLoad()
                .Inverse();

            SchemaAction.Update();
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