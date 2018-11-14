using Cloudents.Admin2.Models;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


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
            var command = new DistributeTokensCommand(model.UserId, model.Tokens, ActionType.None, model.TransactionType);
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
            List<string> emailList = new List<string>();
            foreach (var id in model.Ids)
            {
                var userDataByIdQuery = new UserDataByIdQuery(id);
                var userDataTask = _queryBus.QueryAsync<User>(userDataByIdQuery, token);
                if (model.DeleteUserQuestions)
                {

                    var answersQuery = new UserDataByIdQuery(id);
                    var answersInfo = await _queryBus.QueryAsync<SuspendUserDto>(answersQuery, token);

                    foreach (var question in answersInfo.Questions)
                    {
                        var deleteQuestionCommand = new DeleteQuestionCommand(question);
                        await commandBus.DispatchAsync(deleteQuestionCommand, token).ConfigureAwait(false);
                    }

                    foreach (var answer in answersInfo.Answers)
                    {
                        var deleteAnswerCommand = new DeleteAnswerCommand(answer);
                        await commandBus.DispatchAsync(deleteAnswerCommand, token).ConfigureAwait(false);
                    }

                }
                var command = new SuspendUserCommand(id);
                await commandBus.DispatchAsync(command, token);
                var userData = await userDataTask;
                emailList.Add(userData.Email);
            }
            return new SuspendUserResponse() { Email = emailList };
        }

        [HttpPost("ChangeCountry")]
        [ProducesResponseType(200)]
        public async Task ChangeCountryAsync(ChangeCountryrequest model,
            [FromServices] ICommandBus commandBus,
            CancellationToken token)
        {
            
            var command = new ChangeCountryCommand(model.Id, model.Country);
            await commandBus.DispatchAsync(command, token);
        }
    }
}