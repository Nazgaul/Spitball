using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Data;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using QuestionSearch = Cloudents.Core.Entities.Search.Question;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<QuestionSearch>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<AzureSyncBaseDto<QuestionSearch>> update, IEnumerable<long> delete, long version)>
    {

        private const string QuestionAlias = "q";
        private const string UserAlias = "u";
        private const string QuestionSubjectAlias = "qs";

        public QuestionSyncAzureSearchQueryHandler(QueryBuilder queryBuilder, DapperRepository dapperRepository) :
            base(queryBuilder, dapperRepository)
        {
        }



        protected override string VersionSql => $"select * {QueryBuilder.BuildDiffVersionTable<Question>("q", "c", 56123)}";

        protected override string FirstQuery => $@"
select 
{QueryBuilder.BuildProperty<User, QuestionSearch>(UserAlias, t => t.Id, x => x.UserId)},
{QueryBuilder.BuildProperty<User, QuestionSearch>(UserAlias, t => t.Name, x => x.UserName)},
{QueryBuilder.BuildProperty<User, QuestionSearch>(UserAlias, t => t.Image, x => x.UserImage)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Id, x => x.Id)},
(select count(*) from sb.Answer where QuestionId = q.id) AnswerCount,
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Updated, x => x.DateTime)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Attachments, x => x.FilesCount)},
CASE when q.CorrectAnswer_id IS null Then 0 else 1 
      END HasCorrectAnswer,
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Price, x => x.Price)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Text, x => x.Text)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Color, x => x.Color)},
{QueryBuilder.BuildProperty<QuestionSubject, QuestionSearch>(QuestionSubjectAlias, t => t.Text, x => x.SubjectText)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(QuestionAlias, t => t.Subject, x => x.Subject)},
c.Id as TTT, c.SYS_CHANGE_VERSION
{QueryBuilder.BuildInitVersionTable<Question>(QuestionAlias, "c")} 
join {QueryBuilder.BuildTable<User>(UserAlias)} on {QueryBuilder.BuildProperty<Question>(QuestionAlias, t => t.User)}={QueryBuilder.BuildProperty<User>(UserAlias, u => u.Id)}
join {QueryBuilder.BuildTable<QuestionSubject>(QuestionSubjectAlias)} on {QueryBuilder.BuildProperty<Question>(QuestionAlias, t => t.Subject)}={QueryBuilder.BuildProperty<QuestionSubject>(QuestionSubjectAlias, u => u.Id)}";
    }
}