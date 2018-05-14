using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class QuestionMap : SpitballClassMap<Question>
    {
        public QuestionMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Text).Length(8000).Not.Nullable();
            Map(x => x.Price).Not.Nullable();
            Map(x => x.Attachments).Nullable();
            References(x => x.Subject).ForeignKey("Question_AskQuestionSubject").Not.Nullable();
            References(x => x.User).Column("UserId").ForeignKey("Question_User").Not.Nullable();
        }
    }
}
