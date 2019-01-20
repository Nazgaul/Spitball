using Cloudents.Admin2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;


namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]"), ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminUserController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Send to a specific user tokens
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("sendTokens")]
        public async Task<IActionResult> Post(SendTokenRequest model, CancellationToken token)
        {
            var command = new DistributeTokensCommand(model.UserId, model.Tokens);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        /// <summary>
        /// Get a list of user that want to cash out
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("cashOut")]
        public async Task<IEnumerable<CashOutDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<CashOutDto>>(query, token);
        }

        /// <summary>
        /// Suspend a user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="commandBus"></param>
        /// <param name="token"></param>
        /// <response code="200">The User email</response>
        /// <returns>the user email to show on the ui</returns>
        [HttpPost("suspend")]
        [ProducesResponseType(200)]
        public async Task<SuspendUserResponse> SuspendUserAsync(SuspendUserRequest model,
            [FromServices] ICommandBus commandBus,
            CancellationToken token)
        {
            foreach (var id in model.Ids)
            {
                DateTimeOffset lockout;
                switch (model.SuspendTime)
                {
                    case SuspendTime.Day:
                        
                        lockout = DateTimeOffset.Now.AddSeconds(TimeConst.Day);
                        break;
                    case SuspendTime.Week:
                        lockout = DateTimeOffset.Now.AddSeconds(TimeConst.Day * 7);
                        break;
                    case SuspendTime.Undecided:
                        lockout = DateTimeOffset.MaxValue;
                        break;
                    case null:
                        lockout = DateTimeOffset.MaxValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var command = new SuspendUserCommand(id, model.DeleteUserQuestions, lockout);
                await commandBus.DispatchAsync(command, token);
            }
            return new SuspendUserResponse();
        }

        /// <summary>
        /// Get a list of user that have been suspended
        /// </summary>
        /// <param name="token"></param>
        /// <returns>list of user that have been suspended</returns>
        [HttpGet("suspended")]
        public async Task<IEnumerable<SuspendedUsersDto>> GetSuspended(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            return await _queryBus.QueryAsync<IEnumerable<SuspendedUsersDto>>(query, token);
        }

        /// <summary>
        /// UnSuspend a user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <response code="200">The User email</response>
        /// <returns>the user email to show on the ui</returns>
        [HttpPost("unSuspend")]
        [ProducesResponseType(200)]
        public async Task<UnSuspendUserResponse> UnSuspendUserAsync(UnSuspendUserRequest model,
            CancellationToken token)
        {
            foreach (var id in model.Ids)
            {
               
                var command = new UnSuspendUserCommand(id);
                await _commandBus.DispatchAsync(command, token);
            }
            return new UnSuspendUserResponse();
        }

        [HttpPost("country")]
        [ProducesResponseType(200)]
        public async Task ChangeCountryAsync(ChangeCountryRequest model,
            CancellationToken token)
        {
            
            var command = new ChangeCountryCommand(model.Id, model.Country);
            await _commandBus.DispatchAsync(command, token);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        public async Task DeleteUserAsync(long id,
            CancellationToken token)
        {
            var command = new DeleteUserCommand(id);
            await _commandBus.DispatchAsync(command, token);
        }
        
        [HttpGet("info")]
        public async Task<UserInfoDto> GetUserInfo(long userIdentifier, CancellationToken token)
        {
            var query = new AdminUserInfoQuery(userIdentifier);
            return await _queryBus.QueryAsync(query, token);
        }

    }
}