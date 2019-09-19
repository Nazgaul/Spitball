using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(/*Roles = Roles.Admin*/)]
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
            var query = new AdminPaymentsQuery();
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PayAsync([FromBody]PaymentRequest model,
            //[FromServices] PayMeCredentials payMeCredentials,
            [FromServices] TelemetryClient client,
            CancellationToken token)
        {
            try
            {
                var command = new PaymentCommand(model.UserId, model.TutorId, model.StudentPay,
                    model.StudyRoomSessionId);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (HttpRequestException ex)
            {
                client.TrackException(ex,model.AsDictionary());
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deletePayment")]
        public async Task<IActionResult> DeletePaymentAsync(long userId,
            CancellationToken token)
        {
            var command = new DeleteUserPaymentCommand(userId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> DeclinePay(Guid studyRoomSessionId, CancellationToken token)
        {
            var command = new DeclinePaymentCommand(studyRoomSessionId);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }
    }
}