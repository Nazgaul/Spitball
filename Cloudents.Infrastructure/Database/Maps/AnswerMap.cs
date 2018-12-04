using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class AnswerMap : ClassMap<Answer>
    {
        public AnswerMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Text).Length(8000);
            Map(x => x.Attachments).Nullable();
            Map(x => x.Created).Not.Nullable();
            References(x => x.User).Column("UserId").ForeignKey("Answer_User").Not.Nullable();
            References(x => x.Question).Column("QuestionId").ForeignKey("Answer_Question").Not.Nullable();

            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            HasMany(x => x.Transactions)
                //.Cascade()
                .LazyLoad()
                .Inverse();

            SchemaAction.None();
            
        }
    }
}