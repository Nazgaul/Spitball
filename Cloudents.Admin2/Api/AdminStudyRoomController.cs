using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminStudyRoomController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AdminStudyRoomController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<StudyRoomDto>> StudyRoomAsync(CancellationToken token)
        {
            var query = new AdminStudyRoomQuery(User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpGet("tutors")]
        public async Task<IEnumerable<TutorInfoDto>> TutorsAsync(CancellationToken token)
        {
            var query = new AdminTutorsWithStudyRoomsQuery(User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpGet("bills")]
        public async Task<IEnumerable<SessionBillDto>> BillsAsync(long id, CancellationToken token)
        {
            var query = new AdminTutorSessionsQuery(id, User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSessionAsync([FromBody] UpdateSessionDurationRequest model, 
            [FromServices] ICommandBus commandBus,
            CancellationToken token)
        {
            try
            {
                var command = new UpdateSessionInfoCommand(model.SessionId, model.Minutes);
                await commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
           
        }
    }
}
