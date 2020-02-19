using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IEnumerable<PaymentResponse>> GetPayments(CancellationToken token)
        {
            var query = new PaymentsQuery(User.GetCountryClaim());
            var result = await _queryBus.QueryAsync(query, token);
            return result.Select(s => new PaymentResponse()
            {
                StudyRoomSessionId = s.StudyRoomSessionId,
                Price = s.Price,
                IsSellerKeyExists = s.IsSellerKeyExists,
                IsPaymentKeyExists = s.IsPaymentKeyExists,
                TutorId = s.TutorId,
                TutorName = s.TutorName,
                UserId = s.UserId,
                UserName = s.UserName,
                Created = s.Created,
                Duration = s.Duration.TotalMinutes
            });
        }

        [HttpGet("{id}")]
        public async Task<PaymentDetailDto> GetPayment(Guid id, CancellationToken token)
        {

            var query = new PaymentBySessionIdQuery(id);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PayAsync([FromBody]PaymentRequest model,
            [FromServices] PayMeCredentials payMeCredentials,
            [FromServices] TelemetryClient client,
            CancellationToken token)
        {
            try
            {
                var command = new PaymentCommand(model.UserId, model.TutorId, model.StudentPay, model.SpitballPay,
                    model.StudyRoomSessionId, payMeCredentials.BuyerKey);
                await _commandBus.DispatchAsync(command, token);

                return Ok();
            }
            catch (HttpRequestException ex)
            {
                client.TrackException(ex, model.AsDictionary());
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete user pay method 
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
        public async Task<IActionResult> DeclinePay(Guid studyRoomSessionId, CancellationToken token)
        {
            var command = new DeclinePaymentCommand(studyRoomSessionId);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }
    }
}