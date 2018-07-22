using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data.Maps
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
            Map(x => x.Created).Not.Nullable();
            References(x => x.Subject).ForeignKey("Question_AskQuestionSubject").Not.Nullable();
            References(x => x.User).Column("UserId").ForeignKey("Question_User").Not.Nullable();
            //HasOne(x => x.CorrectAnswer).Not.ForeignKey();
            References(x => x.CorrectAnswer).ForeignKey("Question_Answer").Nullable();
            HasMany(x => x.Answers)
                .Inverse()
                //TODO: this is generate exception when creating new answer. need to figure it out
            //    .Not.KeyNullable()
            //    .Not.KeyUpdate()
                .Cascade.AllDeleteOrphan();
            HasMany(x => x.Transactions)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
