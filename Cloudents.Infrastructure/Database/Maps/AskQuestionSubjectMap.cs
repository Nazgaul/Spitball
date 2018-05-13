using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    public class QuestionSubjectMap : ClassMap<QuestionSubject>
    {
        public QuestionSubjectMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(e => e.Subject);
            ReadOnly();
        }
    }

    [UsedImplicitly]
    public class QuestionMap:ClassMap<Question>
    {
        public QuestionMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Text).Length(8000);
            Map(x => x.Price);
            Map(x => x.Attachments);
            References(x => x.Subject).ForeignKey("Question-AskQuestionSubject");

        }
    }
}
