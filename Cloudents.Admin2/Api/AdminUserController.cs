using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Microsoft.AspNetCore.Authorization;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]"), ApiController]
    [Authorize]
    public class AdminUserController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly IQueueProvider _queueProvider;


        public AdminUserController(ICommandBus commandBus, IQueryBus queryBus,
            IDocumentDirectoryBlobProvider blobProvider, IQueueProvider queueProvider)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _blobProvider = blobProvider;
            _queueProvider = queueProvider;
        }

        /// <summary>
        /// Send to a specific user tokens
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("sendTokens")]
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task<IActionResult> Post(SendTokenRequest model, CancellationToken token)
        {
            var command = new DistributeTokensCommand(model.UserId, model.Tokens);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("cashOut/approve")]
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task<IActionResult> ApprovePost(ApproveCashOutRequest model, CancellationToken token)
        {
            var command = new ApproveCashOutCommand(model.TransactionId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("cashOut/decline")]
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task<IActionResult> DeclinePost(DeclineCashOutRequest model, CancellationToken token)
        {
            var command = new DeclineCashOutCommand(model.TransactionId, model.Reason);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        /// <summary>
        /// Get a list of user that want to cash out
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("cashOut")]
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task<IEnumerable<CashOutDto>> Get(CancellationToken token)
        {
            var query = new AdminCashOutQuery(User.GetCountryClaim());
            return await _queryBus.QueryAsync(query, token);
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
        [Authorize(/*Roles = Roles.Admin*/)]
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

                var command = new SuspendUserCommand(id, lockout, model.Reason);
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
            var country = User.GetCountryClaim();
            var query = new SuspendedUsersQuery(country);
            return await _queryBus.QueryAsync(query, token);
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
        [Authorize(/*Roles = Roles.Admin*/)]
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
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task ChangeCountryAsync(ChangeCountryRequest model,
            CancellationToken token)
        {

            var command = new ChangeCountryCommand(model.Id, model.Country);
            await _commandBus.DispatchAsync(command, token);
        }



        [HttpPost("verify")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(/*Roles = Roles.Admin*/)]
        public async Task<IActionResult> VerifySmsAsync(PhoneConfirmRequest model,
            CancellationToken token)
        {

            var phoneCommand = new ConfirmPhoneNumberCommand(model.Id);
            var registrationBonusCommand = new FinishRegistrationCommand(model.Id);
            try
            {
                await _commandBus.DispatchAsync(phoneCommand, token);
                await _commandBus.DispatchAsync(registrationBonusCommand, token);
            }

            catch
            {
                return BadRequest();
            }

            return Ok(new
            {
                model.Id
            });

        }

        


        [HttpGet("info")]
        public async Task<ActionResult<UserDetailsDto>> GetUserDetails(string userIdentifier, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var regex = new Regex("^[0-9]+$");
            if (userIdentifier[0] == '0' && regex.IsMatch(userIdentifier))
            {
                userIdentifier = $"+972{userIdentifier.Remove(0, 1)}";
            }

            var query = new AdminUserDetailsQuery(userIdentifier, country);

            var res = await _queryBus.QueryAsync(query, token);
            if (res == null)
            {
                return NotFound();
            }
            return res;
        }

        [HttpGet("questions")]
        public async Task<IEnumerable<UserQuestionsDto>> GetUserQuestionsDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            AdminUserQuestionsQuery query = new AdminUserQuestionsQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("answers")]
        public async Task<IEnumerable<UserAnswersDto>> GetUserAnswersDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            AdminUserAnswersQuery query = new AdminUserAnswersQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }

        //[HttpGet("chat")]
        //public async Task<IEnumerable<ConversationDto>> ConversationAsync(long id, CancellationToken token)
        //{
        //    var query = new AdminConversationsQuery(id, 0);
        //    return await _queryBus.QueryAsync(query, token);
        //}

        [HttpGet("sessions")]
        public async Task<IEnumerable<SessionDto>> SessionsAsync(long id, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new AdminSessionsQuery(id, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("purchased")]
        public async Task<IEnumerable<UserPurchasedDocsDto>> GetUserPurchasedDocsDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new AdminUserPurchasedDocsQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("sold")]
        public async Task<IEnumerable<UserSoldItemsDto>> GetUserSoldDocsDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new AdminUserSoldDocsQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("documents")]
        public async Task<IEnumerable<UserDocumentsDto>> GetUserInfo(long id, int page, [FromServices] IBlobProvider blobProvider,
             CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new AdminUserDocumentsQuery(id, page, country);


            var retVal = (await _queryBus.QueryAsync(query, token)).ToList();
            var tasks = new Lazy<List<Task>>();

            foreach (var document in retVal)
            {
                var files = await _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token);
                var file = files.FirstOrDefault();
                if (file != null)
                {
                    document.Preview =
                        blobProvider.GeneratePreviewLink(file,
                            TimeSpan.FromMinutes(20));

                    document.SiteLink = Url.RouteUrl("DocumentDownload", new { id = document.Id });
                }
                else
                {
                    var t = _queueProvider.InsertBlobReprocessAsync(document.Id);
                    tasks.Value.Add(t);
                }

            }

            return retVal;
        }

        [HttpGet("usersFlags")]
        public async Task<UsersFlagsResponse> GetFlags(int minFlags, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new AdminUserFlagsOthersQuery(minFlags, page, country);
            var res = await _queryBus.QueryAsync(query, token);
            return new UsersFlagsResponse { Flags = res.Item1, Rows = res.Item2 };
        }

        [HttpPut("phone")]
        public async Task<IActionResult> UpdatePhoneAsync(
                [FromBody]UpdatePhoneRequest model, CancellationToken token)
        {
            var command = new UpdatePhoneCommand(model.UserId, model.NewPhone);
            try
            {
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                return Conflict();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut("name")]
        public async Task<IActionResult> UpdatePhoneAsync(
                [FromBody]UpdateNameRequest model, CancellationToken token)
        {
            var command = new UpdateNameCommand(model.UserId, model.FirstName, model.LastName);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}