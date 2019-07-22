using Cloudents.Command;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using System.Threading;
using Cloudents.Query.Query.Admin;
using Cloudents.Admin2.Models;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;
using Cloudents.Core.Enum;
using Microsoft.AspNetCore.Authorization;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminUniversityController: ControllerBase
    {
       private readonly IQueryBus _queryBus;
       private readonly ICommandBus _commandBus;
        private readonly IUniversitySearch _universityProvider;
        private readonly DapperRepository _dapperRepository;

        public AdminUniversityController(IQueryBus queryBus, ICommandBus commandBus, 
            IUniversitySearch universityProvider, DapperRepository dapperRepository)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _universityProvider = universityProvider;
            _dapperRepository = dapperRepository;
        }


        //TODO: Fix this and make it work in proper CQRS architecture
        [HttpPost("migrate")]
        public async Task<IActionResult> MigrateUniversity([FromBody] MigrateUniversityRequest model,
            CancellationToken token)
        {
            const string update = @"update sb.[user] set UniversityId2 = @NewUni where UniversityId2 = @OldUni;
                                update sb.Document set UniversityId = @NewUni where UniversityId = @OldUni;
                                update sb.Question set UniversityId = @NewUni where UniversityId = @OldUni;
                                delete from sb.University where id =  @OldUni;";



           await _dapperRepository.WithConnectionAsync(async f => await f.ExecuteAsync(update, new
            {
                NewUni = model.UniversityToKeep,
                OldUni = model.UniversityToRemove
            }), token);
            /*var command = new MigrateUniversityCommand(model.UniversityToKeep, model.UniversityToRemove);
            await _commandBus.DispatchAsync(command, token);*/
            return Ok();
        }


        [HttpGet("newUniversities")]
        public async Task<IEnumerable<PendingUniversitiesDto>> GetNewUniversities([FromQuery] UniversitiesRequest model
            , CancellationToken token)
        {
            var query = new AdminUniversitiesQuery(model.Country, model.State.GetValueOrDefault(ItemState.Pending));
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpGet("allUniversities")]
        public async Task<IEnumerable<AllUniversitiesDto>> GetAllUniversities(CancellationToken token)
        {
            var query = new AdminAllUniversitiesEmptyQuery();
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }


        /// <summary>
        /// Get list of universities
        /// </summary>
        /// <param name="university">university</param>
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [Route("search")]
        [HttpGet]

        public async Task<UniversitySearchDto> GetAsync([FromQuery(Name = "university")]string university,
            CancellationToken token)
        {
            //Only IL Need to think about it
            var result = await _universityProvider.SearchAsync(university, 0,
                null, token);
            return result;
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveUniversity([FromBody] ApproveUniversityRequest model,
                CancellationToken token)
        {
            var command = new ApproveUniversityCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        //TODO: Fix this and make it work in proper CQRS architecture 
        [HttpDelete("{id}")]
        public async Task<IActionResult> ApproveCourse(Guid id,
                CancellationToken token)
        {
            const string sql = @"update sb.[user] set UniversityId2 = null where UniversityId2 = @OldUni;
                                delete from sb.University where id =  @OldUni;";

            await _dapperRepository.WithConnectionAsync(async f => await f.ExecuteAsync(sql, new
            {
                OldUni = id
            }), token);

            return Ok();
        }

        [HttpPost("rename")]
        public async Task<IActionResult> RenameUniversity([FromBody] RenameUniversityRequest model,
        CancellationToken token)
        {
            var command = new RenameUniversityCommand(model.UniversityId, model.NewName);
             await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
