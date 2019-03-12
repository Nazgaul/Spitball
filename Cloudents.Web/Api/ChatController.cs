using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Query;
using Cloudents.Query.Query;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;

        public ChatController(ICommandBus commandBus, UserManager<RegularUser> userManager, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<ChatUserDto>> Get(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var result = await _queryBus.QueryAsync(new ChatConversationsQuery(userId), token);
            return result;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            //specific conversation
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ChatMessageRequest model, CancellationToken token)
        {
            var command = new SendMessageCommand(model.Message, _userManager.GetLongUserId(User),
                new[] { model.OtherUser }, null, null);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
