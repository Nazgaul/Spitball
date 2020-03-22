using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminTutorController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;


        public AdminTutorController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<PendingTutorsDto>> GetPendingTutorsAsync([FromServices] IUrlBuilder urlBuilder, CancellationToken token)
        {
            var query = new PendingTutorsQuery(User.GetCountryClaim());
            var res = await _queryBus.QueryAsync(query, token);
            return res.Select(item =>
            {
                item.Image = urlBuilder.BuildUserImageEndpoint(item.Id, item.Image);
                return item;
            });
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveTutorAsync([FromBody] ApproveTutorRequest model,
        CancellationToken token)
        {
            var command = new ApproveTutorCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTutorAsync(long id,
                CancellationToken token)
        {
            var command = new DeleteTutorCommand(id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("Price")]
        public async Task<IActionResult> ChangePriceAsync([FromBody] ChangePriceRequest model, CancellationToken token)
        {
            var command = new ChangeTutorPriceCommand(model.TutorId, model.Price);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("suspend")]
        [Authorize]
        public async Task<IActionResult> SuspendTutorAsync([FromBody] SuspendTutorRequest model, CancellationToken token)
        {
            var command = new SuspendTutorCommand(model.TutorId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("unsuspend")]
        [Authorize]
        public async Task<IActionResult> SuspendTutorAsync([FromBody] UnSuspendTutorRequest model, CancellationToken token)
        {
            var command = new UnSuspendTutorCommand(model.TutorId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [Route("search")]
        [HttpGet]
        //[Authorize(Policy = Policy.IsraelUser)]
        public async Task<IEnumerable<TutorDto>> GetAsync([FromQuery] TutorSearchRequest model,
           CancellationToken token)
        {
            var adminCountry = User.GetCountryClaim();
            var country = adminCountry ?? model.Country;
            
            var query = new TutorSearchQuery(model.Term, model.State, 0, country);
            var result = await _queryBus.QueryAsync(query, token);
            return result;
        }
    }
}
