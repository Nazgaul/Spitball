using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserInfoQueryHandler : IQueryHandler<AdminUserInfoQuery, UserInfoDto>
    {
        private readonly DapperRepository _dapper;



        public AdminUserInfoQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<UserInfoDto> GetAsync(AdminUserInfoQuery query, CancellationToken token)
        {
                var result = await _dapper.WithConnectionAsync(async connection =>
                {
                    using (var grid = connection.QueryMultiple(@"
                select U.Id, U.Name, Email, PhoneNumberHash as PhoneNumber, Un.Name as University, U.Country, U.Score, U.FraudScore, count(distinct T.Id) as ReferredCount, U.Balance, 
	            case when U.LockOutEnd is null or U.LockOutEnd < getutcdate() then 1
	            else 0 end as IsActive
                            from sb.[User] U
                            join sb.University Un
	                            on U.UniversityId2 = Un.Id
                            left join sb.[Transaction] T
	                            on U.Id = T.[User_id] and T.[Action] = 'ReferringUser'
                            where U.Id = @Id
                group by U.Id, U.Name, Email, PhoneNumberHash, Un.Name, U.Country, U.Score, U.FraudScore, U.Balance, 
                case when U.LockOutEnd is null or U.LockOutEnd < getutcdate() then 1
	                else 0 end;

                select Id, Text, Created, [State]
                from sb.Question 
                where UserId = @Id;

                select A.Id, A.Text, A.Created, A.QuestionId, Q.Text as QuestionText, A.[State]
                from sb.Answer A
                join sb.Question Q
	                on A.QuestionId = Q.Id
                where A.UserId = @Id;

                select D.Id, D.Name, D.CreationTime as Created, U.Name as University, D.CourseName as Course, D.Price, D.[state]
                from sb.Document D
                join sb.University U
	                on D.UniversityId = U.Id
                where UserId = @Id;", new {Id = query.UserId}))
                    {


                        var user = await grid.ReadFirstAsync<UserDetailsDto>();
                        var questions = await grid.ReadAsync<UserQuestionsDto>();
                        var answers = await grid.ReadAsync<UserAnswersDto>();
                        var documents = await grid.ReadAsync<UserDocumentsDto>();
                        return (user, questions, answers, documents);
                    }
                }, token);

            var destination = new UserInfoDto()
            {
                User = result.user,
                Questions = result.questions,
                Answers = result.answers,
                Documents = result.documents
            };
            return destination;
            /*var orderDto = _mapper.Map<(UserDto, IEnumerable<UserQuestionsDto>,
                        IEnumerable<UserAnswersDto>, IEnumerable<UserDocumentsDto>), UserInfoDto>(result);
                        
            return _mapper.Map(result, destination);
            */
        }
    }
}
