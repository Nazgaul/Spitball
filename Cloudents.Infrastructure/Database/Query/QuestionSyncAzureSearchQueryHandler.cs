using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using QuestionSearch = Cloudents.Core.Entities.Search.Question;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<QuestionSearch>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<QuestionSearch> update, IEnumerable<long> delete, long version)>
    {
        private readonly QueryBuilder QueryBuilder;
        private readonly QueryBuilder3<Question> _question;
        private readonly QueryBuilder3<User> _user;
        private readonly QueryBuilder3<QuestionSubject> _questionSubject;
        private readonly QueryBuilder3<Answer> _answer;

        private readonly FluentQueryBuilder _queryBuilder;


        private const string QuestionAlias = "q";
        private const string UserAlias = "u";
        private const string QuestionSubjectAlias = "qs";

        public QuestionSyncAzureSearchQueryHandler(
            QueryBuilder3<Question>.Factory queryBuilderQuestion,

            QueryBuilder3<User>.Factory queryBuilderUser,
            QueryBuilder3<QuestionSubject>.Factory queryBuilderQuestionSubject,
            QueryBuilder3<Answer>.Factory answerQueryBuilder,



            QueryBuilder queryBuilder, ReadonlyStatelessSession session, FluentQueryBuilder queryBuilder1) :
            base(session)
        {
            QueryBuilder = queryBuilder;
            _queryBuilder = queryBuilder1;
            _question = queryBuilderQuestion.Invoke("q");
            _user = queryBuilderUser.Invoke("u");
            _answer = answerQueryBuilder.Invoke("a");
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


        protected string FirstQuery3 => $@"select 
b.Id as UserId,
b.Name as UserName,
b.Image as UserImage,
a.Id as Id,
(select count(*) from sb.Answer where QuestionId = a.id) AnswerCount,
a.Updated as DateTime,
a.Attachments as FilesCount,
CASE when a.CorrectAnswer_id IS null Then 0 else 1 
      END HasCorrectAnswer,
a.Price as Price,
a.Text as Text,
a.Color as Color,
c.Subject as SubjectText,
a.Subject_id as Subject,
c2.Id,c2.SYS_CHANGE_VERSION

 FROM sb.[Question] As a CROSS APPLY CHANGETABLE (VERSION sb.[Question], (Id), (a.Id)) AS c2  
            join sb.[User] As b
            on b.Id=a.UserId

            join sb.[QuestionSubject] As c
            on c.Id=a.Subject_id";



        protected string FirstQuery333 => $@"select 
{_user.Column(x => x.Id)} as UserId,
{_user.Column(x => x.Name)} as UserName,
{_user.Column(x => x.Image)} as UserImage,
{_question.Column(x => x.Id)} as Id,
(select count(*) from {_answer.TableAlias} where {_answer.Column(x => x.Question)} = {_question.Column(x => x.Id)}) AnswerCount,
{_question.Column(x => x.Updated)} as DateTime,
{_question.Column(x => x.Attachments)} as FilesCount,
CASE when {_question.Column(x => x.CorrectAnswer)} IS null Then 0 else 1 
      END HasCorrectAnswer,
{_question.Column(x => x.Price)} as Price,
{_question.Column(x => x.Text)} as Text,
{_question.Column(x => x.Color)} as Color,
{_questionSubject.Column(x => x.Text)} as SubjectText,
{_question.Column(x => x.Subject)} as Subject,
c2.Id,c2.SYS_CHANGE_VERSION

 FROM {_question.TableAlias} CROSS APPLY CHANGETABLE (VERSION {_question.Table}, (Id), ({_question.Column(b => b.Id)})) AS c2  
            join {_user.TableAlias}
            on {_user.Column(x => x.Id)}={_question.Column(x => x.User)}

            join {_questionSubject.TableAlias}
            on {_questionSubject.Column(x => x.Id)}={_question.Column(x => x.Subject)}";

        protected override string FirstQuery
        {
            get
            {
                var qb = _queryBuilder;
                qb.AddInitTable<Question>();
                qb.AddCustomTable(
                        $"CROSS APPLY CHANGETABLE (VERSION {qb.Table<Question>()}, (Id), ({qb.Column<Question>(x => x.Id)})) AS c2")
                    .AddJoin<Question, User>(q => q.User, u => u.Id)
                    .AddJoin<Question, QuestionSubject>(q => q.Subject, qs => qs.Id)

                    .AddSelect<User>(x => x.Id, nameof(QuestionSearch.UserId))
                    .AddSelect<User>(x => x.Name, nameof(QuestionSearch.UserName))
                    .AddSelect<User>(x => x.Image, nameof(QuestionSearch.UserImage))
                    .AddSelect<Question>(x => x.Id, nameof(QuestionSearch.Id))
                    .AddSelect(
                        $"(select count(*) from {qb.Table<Answer>()} where {qb.Column<Answer>(x => x.Question)} = {qb.ColumnAlias<Question>(x => x.Id)}) {nameof(QuestionSearch.AnswerCount)}")

                    .AddSelect<Question>(x => x.Updated, nameof(QuestionSearch.DateTime))
                    .AddSelect<Question>(x => x.Attachments, nameof(QuestionSearch.FilesCount))
                    .AddSelect(
                        $"CASE when {qb.ColumnAlias<Question>(x => x.CorrectAnswer)} IS null Then 0 else 1  END {nameof(QuestionSearch.HasCorrectAnswer)}")
                    .AddSelect<Question>(x => x.Price, nameof(QuestionSearch.Price))
                    .AddSelect<Question>(x => x.Text, nameof(QuestionSearch.Text))
                    .AddSelect<Question>(x => x.Color, nameof(QuestionSearch.Color))
                    .AddSelect<QuestionSubject>(x => x.Text, nameof(QuestionSearch.SubjectText))
                    .AddSelect<QuestionSubject>(x => x.Id, nameof(QuestionSearch.Subject))
                    .AddSelect("c.*");

                return qb;
            }
        }
    }
}