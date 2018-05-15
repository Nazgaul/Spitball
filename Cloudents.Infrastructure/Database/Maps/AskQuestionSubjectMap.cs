using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class QuestionSubjectMap : SpitballClassMap<QuestionSubject>
    {
        public QuestionSubjectMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(e => e.Subject);
            ReadOnly();
        }
    }
}
