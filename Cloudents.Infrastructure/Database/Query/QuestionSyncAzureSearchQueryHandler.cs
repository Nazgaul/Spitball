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

        private readonly FluentQueryBuilder _queryBuilder;



        public QuestionSyncAzureSearchQueryHandler(
            ReadonlyStatelessSession session, FluentQueryBuilder queryBuilder) :
            base(session)
        {
            _queryBuilder = queryBuilder;
        }



        protected override FluentQueryBuilder VersionSql
        {
            get
            {
                var qb = _queryBuilder;
                qb.InitTable<Question>();
                qb.CustomTable(
                    $"right outer join CHANGETABLE (CHANGES {qb.Table<Question>()}, {qb.Param("Version")}) AS c ON {qb.ColumnAlias<Question>(q => q.Id)} = c.id");
                SimilarQuery(qb);
                return qb;
            }
        }

        private void SimilarQuery(FluentQueryBuilder qb)
        {

            qb.Join<Question, User>(q => q.User, u => u.Id)
             .Join<Question, QuestionSubject>(q => q.Subject, qs => qs.Id);
            qb.Select<User>(x => x.Id, nameof(QuestionSearch.UserId))
                .Select<User>(x => x.Name, nameof(QuestionSearch.UserName))
                .Select<User>(x => x.Image, nameof(QuestionSearch.UserImage))
                .Select<Question>(x => x.Id, nameof(QuestionSearch.Id))
                .Select(
                    $"(select count(*) from {qb.Table<Answer>()} where {qb.Column<Answer>(x => x.Question)} = {qb.ColumnAlias<Question>(x => x.Id)}) {nameof(QuestionSearch.AnswerCount)}")
                .Select<Question>(x => x.Updated, nameof(QuestionSearch.DateTime))
                .Select<Question>(x => x.Attachments, nameof(QuestionSearch.FilesCount))
                .Select(
                    $"CASE when {qb.ColumnAlias<Question>(x => x.CorrectAnswer)} IS null Then 0 else 1  END {nameof(QuestionSearch.HasCorrectAnswer)}")
                .Select<Question>(x => x.Price, nameof(QuestionSearch.Price))
                .Select<Question>(x => x.Text, nameof(QuestionSearch.Text))
                .Select<Question>(x => x.Color, nameof(QuestionSearch.Color))
                //.Select<QuestionSubject>(x => x.Text, nameof(QuestionSearch.SubjectText))
                .Select<QuestionSubject>(x => x.Id, nameof(QuestionSearch.Subject))
                .Select("c.*")
                .AddOrder<Question>(q => q.Id)
                .Paging("PageSize", "PageNumber");
        }

        protected override FluentQueryBuilder FirstQuery
        {
            get
            {
                var qb = _queryBuilder;
                qb.InitTable<Question>();
                qb.CustomTable(
                    $"CROSS APPLY CHANGETABLE (VERSION {qb.Table<Question>()}, (Id), ({qb.Column<Question>(x => x.Id)})) AS c");
                SimilarQuery(qb);
                ////.Join<Question, User>(q => q.User, u => u.Id)
                ////.Join<Question, QuestionSubject>(q => q.Subject, qs => qs.Id)

                //.Select<User>(x => x.Id, nameof(QuestionSearch.UserId))
                //.Select<User>(x => x.Name, nameof(QuestionSearch.UserName))
                //.Select<User>(x => x.Image, nameof(QuestionSearch.UserImage))
                //.Select<Question>(x => x.Id, nameof(QuestionSearch.Id))
                //.Select(
                //    $"(select count(*) from {qb.Table<Answer>()} where {qb.Column<Answer>(x => x.Question)} = {qb.ColumnAlias<Question>(x => x.Id)}) {nameof(QuestionSearch.AnswerCount)}")

                //.Select<Question>(x => x.Updated, nameof(QuestionSearch.DateTime))
                //.Select<Question>(x => x.Attachments, nameof(QuestionSearch.FilesCount))
                //.Select(
                //    $"CASE when {qb.ColumnAlias<Question>(x => x.CorrectAnswer)} IS null Then 0 else 1  END {nameof(QuestionSearch.HasCorrectAnswer)}")
                //.Select<Question>(x => x.Price, nameof(QuestionSearch.Price))
                //.Select<Question>(x => x.Text, nameof(QuestionSearch.Text))
                //.Select<Question>(x => x.Color, nameof(QuestionSearch.Color))
                ////.Select<QuestionSubject>(x => x.Text, nameof(QuestionSearch.SubjectText))
                //.Select<QuestionSubject>(x => x.Id, nameof(QuestionSearch.Subject))
                //.Select("c.*");

                return qb;
            }
        }
    }
}