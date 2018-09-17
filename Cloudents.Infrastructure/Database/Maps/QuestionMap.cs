﻿using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class QuestionMap : SpitballClassMap<Question>
    {
        public QuestionMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='{nameof(Question)}'");
            Map(x => x.Text).Length(8000).Not.Nullable();
            Map(x => x.Price).Not.Nullable();
            Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable().Not.Update();
            Map(x => x.Updated).Not.Nullable();
            Map(x => x.Color);
            References(x => x.Subject).ForeignKey("Question_AskQuestionSubject").Not.Nullable();
            References(x => x.User).Column("UserId").ForeignKey("Question_User").Not.Nullable();
            //HasOne(x => x.CorrectAnswer).Not.ForeignKey();
            References(x => x.CorrectAnswer).ForeignKey("Question_Answer").Nullable();
            HasMany(x => x.Answers)
                .Inverse()
                .ExtraLazyLoad()
                //TODO: this is generate exception when creating new answer. need to figure it out
                //    .Not.KeyNullable()
                //    .Not.KeyUpdate()
                .Cascade.AllDeleteOrphan();

            //DO NOT PUT CASCASE DELETE ORAPHAN SINCE WE HANDLE THIS ON CODE
            HasMany(x => x.Transactions)
                .Cascade.Delete()
                .LazyLoad()
                .Inverse();
        }
    }

    //public class QuestionChangeTableMapping : SpitballClassMap<Question>
    //{
    //    public QuestionChangeTableMapping()
    //    {
            
    //    }
    //}
}
