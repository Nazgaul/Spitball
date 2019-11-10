using Cloudents.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Command.Command.Admin;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminCouponController : ControllerBase
    {
        private readonly ICommandBus _commandBus;

        public AdminCouponController(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        // GET: api/<controller>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>
        [HttpPost]
        public async Task Post([FromBody]CouponRequest model, CancellationToken token)
        {
            var command = new CreateCouponCommand(model.Code,
                model.CouponType,
                model.TutorId,
                model.Value,
                model.Expiration,
                model.Description,
                model.Owner,
                model.Amount,
                model.UsePerUser.GetValueOrDefault(1)
            );
            await _commandBus.DispatchAsync(command, token);
            //return Ok();
        }

        // PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
