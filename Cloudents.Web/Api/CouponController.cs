using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;

        public CouponController(ICommandBus commandBus, IQueryBus queryBus, UserManager<User> userManager)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _userManager = userManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostAsync([FromBody]CouponRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new CreateCouponCommand(model.Code, model.CouponType, userId, model.Value, model.Expiration);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                return BadRequest("This coupon already exists");
            }
            catch (SqlConstraintViolationException)
            {
                return BadRequest("User need to be a tutor");
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Inavlid Value");
            }
            catch (ArgumentException)
            {
                return BadRequest("Value can not be more then 100%");
            }
            return Ok();
        }
    }
}
