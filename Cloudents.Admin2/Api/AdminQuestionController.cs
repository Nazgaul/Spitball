﻿using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminQuestionController : ControllerBase
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;

        public AdminQuestionController(Lazy<ICommandBus> commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <summary>
        /// create a question for fictive user.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model, CancellationToken token)
        {

            var command = new CreateQuestionCommand(model.Course, model.University, model.Text, model.Country.ToString("G"));
            await _commandBus.Value.DispatchAsync(command, token);
            return Ok();
        }

        /// <summary>
        /// Delete question from the system
        /// </summary>
        /// <param name="ids">a list of ids to delete</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteQuestionAsync([FromQuery(Name = "id")]IEnumerable<long> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {

                var command = new DeleteQuestionCommand(id);

                await _commandBus.Value.DispatchAsync(command, token);
            }
            return Ok();
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveQuestionAsync([FromBody] ApproveQuestionRequest model, CancellationToken token)
        {


            var command = new ApproveQuestionCommand(model.Id);
            await _commandBus.Value.DispatchAsync(command, token);

            return Ok();
        }

        /// <summary>
        /// Get a list of question with pending state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("Pending")]
        public async Task<IEnumerable<PendingQuestionDto>> Get(CancellationToken token)
        {

            var query = new AdminPendingQuestionsQuery(User.GetCountryClaim());
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("flagged")]
        public async Task<IEnumerable<FlaggedQuestionDto>> FlagAsync(CancellationToken token)
        {

            var query = new FlaggedQuestionQuery(User.GetCountryClaim());
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost("unFlag")]
        public async Task<ActionResult> UnFlagAnswerAsync([FromBody] UnFlagQuestionRequest model, CancellationToken token)
        {
            var command = new UnFlagQuestionCommand(model.Id);
            await _commandBus.Value.DispatchAsync(command, token);
            return Ok();
        }
    }
}