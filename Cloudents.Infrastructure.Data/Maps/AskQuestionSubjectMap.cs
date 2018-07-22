using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data.Maps
{
    [UsedImplicitly]
    public class QuestionSubjectMap : SpitballClassMap<QuestionSubject>
    {
        public QuestionSubjectMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(e => e.Text).Column("Subject");
            Map(e => e.Order).Column("OrderColumn");
            ReadOnly();
        }
    }
}
