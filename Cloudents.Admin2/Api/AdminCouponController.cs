using System;
using Cloudents.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Exceptions;

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

     

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CouponRequest model, CancellationToken token)
        {
            try
            {
                var command = new CreateCouponCommand(model.Code,
                    model.CouponType,
                    model.TutorId,
                    model.Value,
                    model.Expiration,
                    model.Description,
                    model.Owner,
                    null,
                    //model.Amount,
                    1
                );
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException e)
            {
                return BadRequest("This coupon already exists");
            }
            return Ok();
        }

     
    }
}
