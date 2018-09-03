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

        public QuestionSyncAzureSearchQueryHandler(QueryBuilder queryBuilder, DapperRepository dapperRepository, ReadonlyStatelessSession session) :
            base(queryBuilder, dapperRepository, session)
        {
        }



        protected override string VersionSql => $"select * {QueryBuilder.BuildDiffVersionTable<Question>("q", "c", 56123)}";

//        protected override string FirstQuery => $@"
//select 
//{QueryBuilder.BuildProperty<User, QuestionSearch>(t => t.Id, x => x.UserId)},
//{QueryBuilder.BuildProperty<User, QuestionSearch>(t => t.Name, x => x.UserName)},
//{QueryBuilder.BuildProperty<User, QuestionSearch>(t => t.Image, x => x.UserImage)},
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Id, x => x.Id)},
//(select count(*) from sb.Answer where QuestionId = a.id) AnswerCount,
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Updated, x => x.DateTime)},
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Attachments, x => x.FilesCount)},
//CASE when a.CorrectAnswer_id IS null Then 0 else 1 
//      END HasCorrectAnswer,
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Price, x => x.Price)},
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Text, x => x.Text)},
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Color, x => x.Color)},
//{QueryBuilder.BuildProperty<QuestionSubject, QuestionSearch>(t => t.Text, x => x.SubjectText)},
//{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Subject, x => x.Subject)},
//c2.*
//{QueryBuilder.BuildInitVersionTable<Question>("c2")} 
//{QueryBuilder.BuildJoin<User,Question >(u => u.Id, q => q.User)}
//{QueryBuilder.BuildJoin<QuestionSubject,Question >(u => u.Id, q => q.Subject)}";


        protected override string FirstQuery => $@"select 
b.Id as Data@UserId,
b.Name as Data@UserName,
b.Image as Data@UserImage,
a.Id as Data@Id,
(select count(*) from sb.Answer where QuestionId = a.id) Data@AnswerCount,
a.Updated as Data@DateTime,
a.Attachments as Data@FilesCount,
CASE when a.CorrectAnswer_id IS null Then 0 else 1 
      END Data@HasCorrectAnswer,
a.Price as Data@Price,
a.Text as Data@Text,
a.Color as Data@Color,
c.Subject as Data@SubjectText,
a.Subject_id as Data@Subject,
c2.Id,c2.SYS_CHANGE_VERSION

 FROM sb.[Question] As a CROSS APPLY CHANGETABLE (VERSION sb.[Question], (Id), (a.Id)) AS c2  

            join sb.[User] As b
            on b.Id=a.UserId

            join sb.[QuestionSubject] As c
            on c.Id=a.Subject_id";
    }
}