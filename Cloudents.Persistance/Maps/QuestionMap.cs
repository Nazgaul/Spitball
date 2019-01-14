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
            Map(x => x.Language).Length(10);
            Map(x => x.Subject).Column("Subject_id").CustomType<int>();

            References(x => x.User).Column("UserId")
                .ForeignKey("Question_User").Not.Nullable();
            References(x => x.CorrectAnswer).ForeignKey("Question_Answer").Nullable();
            HasMany(x => x.Answers)
                .Inverse()
                .ExtraLazyLoad()
                .Cascade.AllDeleteOrphan();

            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            HasMany(x => x.Transactions)
                //.Cascade.()
                .LazyLoad()
                .Inverse();
            
            HasMany(x => x.Votes).KeyColumns.Add("QuestionId").Inverse()
                .Cascade.AllDeleteOrphan();
            SchemaAction.None();
            //DiscriminateSubClassesOnColumn("State");//.Formula($"case when State is Null then 'Ok' else State end");
        }
    }

    public class ItemMap<T> : ClassMap<T> where T : ItemObject
    {
        public ItemMap()
        {
            Map(m => m.VoteCount).Not.Nullable();
            Component(x => x.State);
        }
    }

    public class ItemStateMap : ComponentMap<ItemState2>
    {
        public ItemStateMap()
        {
            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>().Not.Nullable();
            Map(m => m.DeletedOn).Nullable();
            Map(m => m.FlagReason).Nullable();
            References(x => x.FlaggedUser).Column("FlaggedUserId");
        }
    }
}
