using System;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class QuestionMap : ClassMap<Question>
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
            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Map(x => x.Language).Length(5);
            Map(x => x.Subject).Column("Subject_id").CustomType<int>();

            References(x => x.User).Column("UserId").ForeignKey("Question_User").Not.Nullable();
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

            SchemaAction.None();


        }
    }

    //public class QuestionDeleteMap : ClassMap<Question>
    //{
    //    public QuestionDeleteMap()
    //     : base()
    //    {
    //        EntityName("QuestionDelete");
    //        Table("QuestionDelete");


    //        Id(x => x.Id).GeneratedBy.Assigned();
    //        Map(x => x.Text).Length(8000).Not.Nullable();
    //        Map(x => x.Price).Not.Nullable();
    //        Map(x => x.Attachments).Nullable();
    //        Map(x => x.Created).Not.Nullable().Not.Update();
    //        Map(x => x.Updated).Not.Nullable();
    //        Map(x => x.Color);
    //        Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
    //        Map(x => x.Language).Length(5);
    //        Map(x => x.Subject).Column("Subject_id").CustomType<int>();

    //        References(x => x.User).Columns("Id").Column("UserId").Not.Nullable();
    //        References(x => x.CorrectAnswer).Columns("Id").Nullable();

    //        //References(x => x.User).Column("UserId").ForeignKey("Question_User").Not.Nullable();
    //        //References(x => x.CorrectAnswer).ForeignKey("Question_Answer").Nullable();
           

    //        //SchemaAction.None();


    //        SchemaAction.Update();
    //    }
    //}
}
