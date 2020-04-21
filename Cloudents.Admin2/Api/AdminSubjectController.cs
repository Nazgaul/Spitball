using Cloudents.Command;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Query.Admin;
using Cloudents.Admin2.Models;
using Cloudents.Command.Command.Admin;
using Country = Cloudents.Core.Entities.Country;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminSubjectController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;


        public AdminSubjectController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<SubjectDto>> GetAsync(CancellationToken token)
        {
            var country = User.GetSbCountryClaim();
            var query = new SubjectsTranslationQuery(country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSubjectRequest request,
            CancellationToken token)
        {
            var userId = User.GetIdClaim();
            var country = Country.FromCountry(request.Country);
            var command = new CreateSubjectCommand(request.Name, userId, country);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditAsync([FromBody] EditSubjectRequest request,
            CancellationToken token)
        {
            var command = new EditSubjectCommand(request.SubjectId, request.Name);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubjectAsync(long id, CancellationToken token)
        {
            var command = new DeleteSubjectCommand(id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
