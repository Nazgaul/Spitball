using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminShortUrlController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        public AdminShortUrlController(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }


        [HttpPost("url")]
        public async Task<ActionResult> ApproveQuestionAsync([FromBody] AddShortUrlRequest model, CancellationToken token)
        {
            var command = new CreateShortUrlCommand(model.Identifier, model.Destination, model.Expiration);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }
    }
}
