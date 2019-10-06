using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Sync;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.SearchSync
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionSyncAzureSearchQueryHandler : //SyncAzureSearchQueryHandler<QuestionSearchDto>,
        IQueryHandler<SyncAzureQuery,
            (IEnumerable<QuestionSearchDto> update, IEnumerable<string> delete, long version)>
    {

        private readonly IDapperRepository _repository;

        public QuestionSyncAzureSearchQueryHandler(IDapperRepository repository)
        {
            _repository = repository;
        }

        private const string FirstQuery = @"with cte as (
select q.Id as QuestionId,
                           q.Language as Language,
                           coalesce (uni.country,u.Country) as Country,
                           (select count(*) from sb.Answer where QuestionId = q.Id and State = 'Ok') AnswerCount,
                           q.Updated as DateTime, 
                           CASE when q.CorrectAnswer_id IS null Then 0 else 1  END HasCorrectAnswer,
                           q.Text as Text,
                           q.State as State,
c2.Name as Course,
uni.Name as University,
From sb.[Question] q 
                           join sb.[User] u 
                           On u.Id = q.UserId
left join sb.University uni on uni.Id = q.UniversityId
left join sb.Course c2 on q.CourseId = c2.Name
                           Order by q.Id  
                           OFFSET @PageSize * @PageNumber 
                            ROWS FETCH NEXT @PageSize ROWS ONLY

)
select * from 
cte
CROSS APPLY CHANGETABLE (VERSION sb.[Question], (Id), (Id)) AS c;";

        private const string VersionSql = @"select q.Id as QuestionId,
	    q.Language as Language,
	    coalesce (uni.country,u.Country) as Country,
	    (select count(*) from sb.Answer where QuestionId = q.Id and State = 'Ok') AnswerCount,
	    q.Updated as DateTime, 
	    CASE when q.CorrectAnswer_id IS null Then 0 else 1  END HasCorrectAnswer,
	    q.Text as Text,
	    q.State as State,
		C2.Name as Course,
		uni.Name as University,
	    c.* 
From sb.[Question] q 
right outer join CHANGETABLE (CHANGES sb.[question], @Version) AS c ON q.Id = c.id  
join sb.[User] u 
	On u.Id = q.UserId
left join sb.University uni 
	on uni.Id = q.UniversityId
left join sb.Course c2 on q.CourseId = c2.Name
Order by q.Id 
OFFSET @PageSize * @PageNumber 
ROWS FETCH NEXT @PageSize ROWS ONLY";


        private const int PageSize = 200;


        //protected override ILookup<bool, AzureSyncBaseDto<QuestionSearchDto>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<QuestionSearchDto>> result)
        //{
        //    return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D" || p.Data.State.GetValueOrDefault(ItemState.Ok) != ItemState.Ok);
        //}

        public async Task<(IEnumerable<QuestionSearchDto> update, IEnumerable<string> delete, long version)> GetAsync(SyncAzureQuery query, CancellationToken token)
        {
            var sql = query.Version == 0 ? FirstQuery : VersionSql;
            using (var conn = _repository.OpenConnection())
            {
                var result = await conn.QueryAsync<DbResult>(sql, new
                {
                    query.Version,
                    PageNumber = query.Page,
                    PageSize
                });

                var update = new List<QuestionSearchDto>();
                var delete = new List<string>();
                long version = 0;

                foreach (var dbResult in result)
                {
                    version = Math.Max(dbResult.SYS_CHANGE_VERSION, version);
                    if (dbResult.SYS_CHANGE_OPERATION == "D" || dbResult.State != ItemState.Ok)
                    {
                        delete.Add(dbResult.Id.ToString());
                    }
                    else
                    {
                        var state = QuestionFilter.Unanswered;
                        if (dbResult.AnswerCount > 0)
                        {
                            state = QuestionFilter.Answered;
                        }

                        if (dbResult.HasCorrectAnswer.GetValueOrDefault())
                        {
                            state = QuestionFilter.Sold;
                        }


                        update.Add(new QuestionSearchDto
                        {
                            Country = dbResult.Country,
                            Course = dbResult.Course,
                            State = state,
                            DateTime = dbResult.DateTime,
                            Id = dbResult.QuestionId,
                            Language = dbResult.Language ?? "en",
                            Text = dbResult.Text,
                            UniversityName = dbResult.University,
                        });
                    }
                }

                return (update, delete, version);
            }
        }




        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class DbResult
        {
            [Core.Attributes.EntityBind(nameof(Question.Id))]
            public long QuestionId { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.Language))]
            public string Language { get; set; }
            [Core.Attributes.EntityBind(nameof(BaseUser.Country))]
            public string Country { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.Answers))]
            public int AnswerCount { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.Updated))]
            public DateTime DateTime { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.CorrectAnswer))]
            public bool? HasCorrectAnswer { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.Text))]
            public string Text { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.Status.State))]
            public ItemState State { get; set; }
           
            public long SYS_CHANGE_VERSION { get; set; }
            public string SYS_CHANGE_OPERATION { get; set; }
            //public string SYS_CHANGE_COLUMNS { get; set; }
            [Core.Attributes.EntityBind(nameof(Question.Id))]
            public long Id { get; set; }

            [Core.Attributes.EntityBind(nameof(Question.Course.Id))]
            public string Course { get; set; }

            [Core.Attributes.EntityBind(nameof(Question.University.Name))]
            public string University { get; set; }
          
        }
    }
}