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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly UserManager<RegularUser> _userManager;

        public ChatController(ICommandBus commandBus, UserManager<RegularUser> userManager)
        {
            _commandBus = commandBus;
            _userManager = userManager;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
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

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
