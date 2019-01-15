using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserInfoQueryHandler : IQueryHandler<AdminUserInfoQuery, UserInfoDto>
    {
        private readonly DapperRepository _dapper;
        private readonly IMapper _mapper;



        public AdminUserInfoQueryHandler(DapperRepository dapper, IMapper mapper)
        {
            _dapper = dapper;
            _mapper = mapper;
        }

        public async Task<UserInfoDto> GetAsync(AdminUserInfoQuery query, CancellationToken token)
        {
                var result = await _dapper.WithConnectionAsync(async connection =>
                {
                    var grid = connection.QueryMultiple(@"
                select Id, Name, Score from sb.[User] where Id = @Id;
                select Id, Text, Created from sb.Question where UserId = @Id;
                select Id, Text, Created, QuestionId from sb.Answer where UserId = @Id;
                select D.Id, D.Name, D.CreationTime as Created, U.Name as University, D.CourseName as Course, D.Price
                from sb.Document D
                join sb.University U
	                on D.UniversityId = U.Id
                where UserId = @Id;", new {Id = query.UserId } );

                    var user = await grid.ReadFirstAsync<UserDto>();
                    var questions = await grid.ReadAsync<UserQuestionsDto>();
                    var answers = await grid.ReadAsync<UserAnswersDto>();
                    var documents = await grid.ReadAsync<UserDocumentsDto>();
                    return (user, questions, answers, documents);
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
