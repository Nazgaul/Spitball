using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    public class QuestionMap : ItemMap<Question>
    {
        public QuestionMap() 
        {
            DynamicUpdate();
            //https://stackoverflow.com/a/7084697/1235448
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(Question)}'");
            Map(x => x.Text).Length(8000).Not.Nullable();
            Map(x => x.Price).Not.Nullable();
            Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable().Not.Update();
            Map(x => x.Updated).Not.Nullable();
            Map(x => x.Color);
           
            //Component(x => x.Item);
            Map(x => x.Language).Length(5);
            Map(x => x.Subject).Column("Subject_id").CustomType<int>();
           // Map(x => x.AnswerCount).Not.Nullable();

            References(x => x.User).Column("UserId")
                .ForeignKey("Question_User").Not.Nullable()
                .LazyLoad(Laziness.NoProxy); // we need this because of inheritance
            References(x => x.CorrectAnswer).ForeignKey("Question_Answer").Nullable();
            HasMany(x => x.Answers)
                .Inverse()
                .ExtraLazyLoad()
                //TODO: this is generate exception when creating new answer. need to figure it out
                //    .Not.KeyNullable()
                //    .Not.KeyUpdate()
                .Cascade.AllDeleteOrphan();

            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            HasMany(x => x.Transactions)
                //.Cascade.()
                .LazyLoad()
                .Inverse();

            References(x => x.FlaggedUser)
                .Column("FlaggedUserId").ForeignKey("QuestionFlagged_User");
            HasMany(x => x.Votes).KeyColumns.Add("QuestionId").Inverse()
                .Cascade.AllDeleteOrphan();
            SchemaAction.None();
            //DiscriminateSubClassesOnColumn("State");//.Formula($"case when State is Null then 'Ok' else State end");
        }
    }

    public class ItemMap<T> : ClassMap<T>  where T : ItemObject
    {
        public ItemMap()
        {
            Map(x=>x.State).CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            Map(m => m.DeletedOn).Nullable();
            Map(m => m.FlagReason).Nullable();
            Map(m => m.VoteCount).Not.Nullable();
        }
    }
}
