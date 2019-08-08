using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Admin2.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(/*Roles = Roles.Admin*/)]
    //[Authorize(Policy = Policy.IsraelUser)]
    public class AdminPaymentController : Controller
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
        public async Task<IActionResult> PayAsync(PaymentRequest model,
            [FromServices] PayMeCredentials payMeCredentials,
            CancellationToken token)
        {
            var command = new PaymentCommand(model.UserId,model.TutorId,model.StudentPay,model.SpitballPay,model.StudyRoomSessionId, payMeCredentials.BuyerKey);
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