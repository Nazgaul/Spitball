using Cloudents.Command;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using System.Threading;
using Cloudents.Query.Query.Admin;
using Cloudents.Admin2.Models;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
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

        

        //[HttpGet("universities")]
        //public async Task<IEnumerable<NewUniversitiesDto>> GetUniversities(CancellationToken token)
        //{
        //    var query = new AdminEmptyQuery();
        //    var retVal = await _queryBus.QueryAsync<IList<NewUniversitiesDto>> (query, token);
        //    return retVal;
        //}

        //TODO: Fix this and make it work in proper CQRS architecture
        [HttpPost("migrate")]
        public async Task<IActionResult> MigrateUniversity([FromBody] MigrateUniversityRequest model,
            CancellationToken token)
        {
            const string update = @"update sb.[user] set UniversityId2 = @Newuni where UniversityId2 = @OldUni;
                                update sb.Document set UniversityId = @Newuni where UniversityId = @OldUni;
                                update sb.Question set UniversityId = @Newuni where UniversityId = @OldUni;
                                delete from sb.University where id =  @OldUni;";



            var z = await _dapperRepository.WithConnectionAsync(async f =>
            {
                return await f.ExecuteAsync(update, new
                {
                    Newuni = model.UniversityToKeep,
                    OldUni = model.UniversityToRemove
                });

            }, token);
            /*var command = new MigrateUniversityCommand(model.UniversityToKeep, model.UniversityToRemove);
            await _commandBus.DispatchAsync(command, token);*/
            return Ok();
        }


        [HttpGet("newUniversities")]
        public async Task<IEnumerable<PendingUniversitiesDto>> GetNewUniversities(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<PendingUniversitiesDto>>(query, token);
            return retVal;
        }

        [HttpGet("allUniversities")]
        public async Task<IEnumerable<AllUniversitiesDto>> GetAllUniversities(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<AllUniversitiesDto>>(query, token);
            return retVal;
        }


        /// <summary>
        /// Get list of universities
        /// </summary>
        /// <param name="university">university</param>
        /// <param name="profile">Not taken from the api</param>
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [Route("search")]
        [HttpGet]

        public async Task<UniversitySearchDto> GetAsync([FromQuery(Name = "university")]string university,
            CancellationToken token)
        {
            //Only IL Need to think about it
            var result = await _universityProvider.SearchAsync(university,
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

        [HttpDelete("{Id}")]
        public async Task<IActionResult> ApproveCourse(Guid Id,
                CancellationToken token)
        {
            var command = new DeleteUniversityCommand(Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
