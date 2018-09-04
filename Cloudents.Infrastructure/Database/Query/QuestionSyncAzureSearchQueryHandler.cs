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
        private readonly QueryBuilder3<Question> _question;
        private readonly QueryBuilder3<User> _user;
        private readonly QueryBuilder3<QuestionSubject> _questionSubject;
        private readonly QueryBuilder3<Answer> _answer;

        private const string QuestionAlias = "q";
        private const string UserAlias = "u";
        private const string QuestionSubjectAlias = "qs";

        public QuestionSyncAzureSearchQueryHandler(
            QueryBuilder3<Question>.Factory queryBuilderQuestion,

            QueryBuilder3<User>.Factory queryBuilderUser,
            QueryBuilder3<QuestionSubject>.Factory queryBuilderQuestionSubject,
            QueryBuilder3<Answer>.Factory answerQueryBUilder,



            QueryBuilder queryBuilder, DapperRepository dapperRepository, ReadonlyStatelessSession session) :
            base(queryBuilder, dapperRepository, session)
        {
            _question = queryBuilderQuestion.Invoke("q");
            _user = queryBuilderUser.Invoke("u");
            _answer = answerQueryBUilder.Invoke("a");
            _questionSubject = queryBuilderQuestionSubject.Invoke("qs");
        }



        protected override string VersionSql => $"select * {QueryBuilder.BuildDiffVersionTable<Question>("q", "c", 56123)}";

        protected string FirstQuery2 => $@"
select 
{QueryBuilder.BuildProperty<User, AzureSyncBaseDto<QuestionSearch>>(t => t.Id, x => x.Data.UserId)},
{QueryBuilder.BuildProperty<User, QuestionSearch>(t => t.Name, x => x.UserName)},
{QueryBuilder.BuildProperty<User, QuestionSearch>(t => t.Image, x => x.UserImage)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Id, x => x.Id)},
(select count(*) from sb.Answer where QuestionId = a.id) AnswerCount,
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Updated, x => x.DateTime)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Attachments, x => x.FilesCount)},
CASE when a.CorrectAnswer_id IS null Then 0 else 1 
      END HasCorrectAnswer,
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Price, x => x.Price)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Text, x => x.Text)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Color, x => x.Color)},
{QueryBuilder.BuildProperty<QuestionSubject, QuestionSearch>(t => t.Text, x => x.SubjectText)},
{QueryBuilder.BuildProperty<Question, QuestionSearch>(t => t.Subject, x => x.Subject)},
c2.*
{QueryBuilder.BuildInitVersionTable<Question>("c2")} 
{QueryBuilder.BuildJoin<User, Question>(u => u.Id, q => q.User)}
{QueryBuilder.BuildJoin<QuestionSubject, Question>(u => u.Id, q => q.Subject)}";


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



        protected string FirstQuery3 => $@"select 
{_user.Column(x=>x.Id)} as Data@UserId,
{_user.Column(x => x.Name)} as Data@UserName,
{_user.Column(x => x.Image)} as Data@UserImage,
{_question.Column(x=>x.Id)} as Data@Id,
(select count(*) from {_answer.TableAlias} where {_answer.Column(x=>x.Question)} = {_question.Column(x=>x.Id)}) Data@AnswerCount,
{_question.Column(x => x.Updated)} as Data@DateTime,
{_question.Column(x => x.Attachments)} as Data@FilesCount,
CASE when {_question.Column(x => x.CorrectAnswer)} IS null Then 0 else 1 
      END Data@HasCorrectAnswer,
{_question.Column(x => x.Price)} as Data@Price,
{_question.Column(x => x.Text)} as Data@Text,
{_question.Column(x => x.Color)} as Data@Color,
{_questionSubject.Column(x=>x.Text)}as Data@SubjectText,
{_question.Column(x => x.Subject)} as Data@Subject,
c2.Id,c2.SYS_CHANGE_VERSION

 FROM {_question.TableAlias} CROSS APPLY CHANGETABLE (VERSION {_question.Table}, (Id), ({_question.Column(b => b.Id)})) AS c2  
            join {_user.TableAlias}
            on {_user.Column(x => x.Id)}={_question.Column(x => x.User)}

            join {_questionSubject.TableAlias}
            on {_questionSubject.Column(x => x.Id)}={_question.Column(x => x.Subject)}";
    }
}