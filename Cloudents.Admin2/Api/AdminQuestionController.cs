using Cloudents.Admin2.Models;
using Cloudents.Core;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminQuestionController : ControllerBase
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IQueueProvider _queueProvider;

        public AdminQuestionController(Lazy<ICommandBus> commandBus, IQueryBus queryBus, IQueueProvider queueProvider)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _queueProvider = queueProvider;
        }

        /// <summary>
        /// Get the ability to create a question
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateQuestionAsync([FromBody]CreateQuestionRequest model, CancellationToken token)
        {
            var userId = await _queryBus.QueryAsync<long>(new AdminEmptyQuery(), token);
            var message = new NewQuestionMessage(model.SubjectId, model.Text, model.Price, userId);
            await _queueProvider.InsertQuestionMessageAsync(message, token);
            return Ok();
        }

        /// <summary>
        /// Get a list of question subject for ui
        /// </summary>
        /// <returns></returns>
        [HttpGet("subject")]
        [ResponseCache(Duration = TimeConst.Day)]
        public IEnumerable<QuestionSubjectResponse> GetSubjectsAsync()
        {
            var values = QuestionSubjectMethod.GetValues();

            return values.Select(s => new QuestionSubjectResponse((int)s, s.ToString("G")));
        }

        /// <summary>
        /// Delete question from the system
        /// </summary>
        /// <param name="ids">a list of ids to delete</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteQuestionAsync([FromQuery(Name = "id")]IEnumerable<long> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {

                var command = new DeleteQuestionCommand(id);

                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            }
            return Ok();
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveQuestionAsync([FromBody]ApproveRequest model, CancellationToken token)
        {
            foreach (var id in model.Ids)
            {

                var command = new ApproveQuestionCommand(id);
                await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            }
            return Ok();
        }

    }
}