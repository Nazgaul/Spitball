using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Sync;

namespace Cloudents.Query.SearchSync
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class QuestionSyncAzureSearchQueryHandler : SyncAzureSearchQueryHandler<QuestionSearchDto>,
        IQueryHandler<SyncAzureQuery, (IEnumerable<QuestionSearchDto> update, IEnumerable<string> delete, long version)>
    {

        public QuestionSyncAzureSearchQueryHandler(
            QuerySession session) :
            base(session)
        {
        }

        protected override string VersionSql
        {
            get
            {
                var res = @"select q.Id as QuestionId,
	                            q.Language as Language,
	                            u.Country as Country,
	                            q.AnswerCount AnswerCount,
	                            q.Updated as DateTime,
	                            CASE when q.CorrectAnswer_id IS null Then 0 else 1  END HasCorrectAnswer,
	                            q.Text as Text,
	                            q.State as State,
	                            q.Subject_id as Subject,
	                            c.* 
                            From sb.[Question] q  
                            right outer join CHANGETABLE (CHANGES sb.[Question], :Version) AS c ON q.Id = c.id 
                            left join sb.[User] u 
	                            On u.Id = q.UserId
                            Order by q.Id 
                            OFFSET :PageSize * :PageNumber 
                            ROWS FETCH NEXT :PageSize ROWS ONLY";
                return res;
            }
        }

        protected override string FirstQuery
        {
            get
            {
                var res = @"select q.Id as QuestionId,
	                            q.Language as Language,
	                            u.Country as Country,
	                            q.AnswerCount AnswerCount,
	                            q.Updated as DateTime, 
	                            CASE when q.CorrectAnswer_id IS null Then 0 else 1  END HasCorrectAnswer,
	                            q.Text as Text,
	                            q.State as State,
	                             q.Subject_id as Subject,
	                             c.* 
                            From sb.[Question] q 
                            CROSS APPLY CHANGETABLE (VERSION sb.[Question], (Id), (Id)) AS c 
                            left join sb.[User] u 
	                            On u.Id = q.UserId
                            Order by q.Id 
                            OFFSET :PageSize * :PageNumber 
                            ROWS FETCH NEXT :PageSize ROWS ONLY";
                return res;
            }
        }

        protected override int PageSize => 200;


        protected override ILookup<bool, AzureSyncBaseDto<QuestionSearchDto>> SeparateUpdateFromDelete(IEnumerable<AzureSyncBaseDto<QuestionSearchDto>> result)
        {
            return result.ToLookup(p => p.SYS_CHANGE_OPERATION == "D" || p.Data.State.GetValueOrDefault(ItemState.Ok) != ItemState.Ok);
        }
    }
}