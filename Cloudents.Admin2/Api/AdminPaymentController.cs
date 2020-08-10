using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    //[Authorize(Policy = Policy.IsraelUser)]
    public class AdminPaymentController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminPaymentController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        // GET
        [HttpGet]
        public async Task<IEnumerable<PaymentDto>> GetPayments(CancellationToken token)
        {
            var sbCountry = User.GetSbCountryClaim();
            var queryV2 = new SessionPaymentsQueryV2(sbCountry);
            return await _queryBus.QueryAsync(queryV2, token);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetailDto>> GetPayment(Guid id, [FromQuery] long userId, [FromQuery] long tutorId, CancellationToken token)
        {
            var queryV2 = new PaymentBySessionIdV2Query(id, userId, tutorId);
            var result = await _queryBus.QueryAsync(queryV2, token);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PayAsync([FromBody]PaymentRequest model,
            [FromServices] TelemetryClient client,
            CancellationToken token)
        {
            try
            {
                var command = new PaymentCommand(model.UserId, model.TutorId, model.StudentPay, model.SpitballPay,
                    model.StudyRoomSessionId, TimeSpan.FromMinutes(model.AdminDuration));
                await _commandBus.DispatchAsync(command, token);

                return Ok();
            }
            catch (Exception ex)
            {
                client.TrackException(ex, model.AsDictionary());
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete user pay method - payme token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete("deletePayment")]
        public async Task<IActionResult> DeletePaymentAsync(long userId,
            CancellationToken token)
        {
            var command = new DeleteUserPaymentCommand(userId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> DeclinePay(Guid studyRoomSessionId, long userId, CancellationToken token)
        {
            if (studyRoomSessionId == Guid.Empty)
            {
                return BadRequest();
            }
            var command = new PaymentDeclineCommand(studyRoomSessionId, userId);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }


    }
}