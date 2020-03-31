using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Session;
using Cloudents.Query.Users;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]
    public class SalesController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        public SalesController(UserManager<User> userManager, IQueryBus queryBus, ICommandBus commandBus)
        {
            _userManager = userManager;
            _queryBus = queryBus;
            _commandBus = commandBus;
        }


        [HttpGet]
        public async Task<IEnumerable<SaleDto>> GetUserSalesAsync([FromServices] IUrlBuilder urlBuilder, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserSalesByIdQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);

            return result.Select(s =>
            {
                if (s is DocumentSaleDto d)
                {
                    d.Preview = urlBuilder.BuildDocumentThumbnailEndpoint(d.Id);
                    d.Url = Url.DocumentUrl(d.Course, d.Id, d.Name);
                }
                if (s is SessionSaleDto ss)
                {
                    ss.StudentImage = urlBuilder.BuildUserImageEndpoint(ss.StudentId, ss.StudentImage, ss.StudentName);
                }
                return s;
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetSessionDurationAsync([FromBody] SetSessionDurationRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new SetSessionDurationCommand(userId,
                model.SessionId,
                TimeSpan.FromMinutes(model.DurationInMinutes));
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("session/{id}")]
        public async Task<PaymentDetailDto> GetPaymentAsync([FromRoute] Guid id,[FromQuery] long userId, CancellationToken token)
        {
            var tutorId = _userManager.GetLongUserId(User);
            var querySessionV2 = new SessionApprovalQuery(id, userId, tutorId);

            var result = await _queryBus.QueryAsync(querySessionV2, token);
            if (result is null)
            {
                var query = new PaymentBySessionIdQuery(id);
                return await _queryBus.QueryAsync(query, token);
            }

            return result;
        }
    }
}
