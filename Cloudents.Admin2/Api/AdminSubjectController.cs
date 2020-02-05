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
            if (User.GetCountryClaim() != null)
            {
                return null;
            }
            var query = new SubjectsTranslationQuery();
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSubjectRequest request,
            CancellationToken token)
        {
            var command = new CreateSubjectCommand(request.EnSubjectName, request.HeSubjectName);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditAsync([FromBody] EditSubjectRequest request,
            CancellationToken token)
        {
            var command = new EditSubjectCommand(request.SubjectId, request.EnSubjectName, request.HeSubjectName);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorAsync(long id, CancellationToken token)
        {
            var command = new DeleteSubjectCommand(id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
