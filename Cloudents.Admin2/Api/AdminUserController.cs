﻿using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Extension;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]"), ApiController]
    [Authorize]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public class AdminUserController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly IQueueProvider _queueProvider;
        private readonly IUrlBuilder _urlBuilder;


        public AdminUserController(ICommandBus commandBus, IQueryBus queryBus,
            IDocumentDirectoryBlobProvider blobProvider, IQueueProvider queueProvider,
            IUrlBuilder urlBuilder)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _blobProvider = blobProvider;
            _queueProvider = queueProvider;
            _urlBuilder = urlBuilder;
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


        [HttpPost("cashOut/approve")]
        public async Task<IActionResult> ApprovePost(ApproveCashOutRequest model, CancellationToken token)
        {
            var command = new ApproveCashOutCommand(model.TransactionId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("cashOut/decline")]
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
        public async Task<IEnumerable<CashOutDto>> Get(CancellationToken token)
        {
            var query = new CashOutQuery(User.GetCountryClaim());
            return await _queryBus.QueryAsync(query, token);
        }


        /// <summary>
        /// Suspend a user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <response code="200">The User email</response>
        /// <returns>the user email to show on the ui</returns>
        [HttpPost("suspend")]
        [ProducesResponseType(200)]

        public async Task<SuspendUserResponse> SuspendUserAsync(SuspendUserRequest model,
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
                await _commandBus.DispatchAsync(command, token);
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
            var country = User.GetSbCountryClaim();
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
        public async Task DeleteUserAsync([FromRoute] long id, CancellationToken token)
        {
            var userId = User.GetIdClaim();
            var command = new DeleteUserCommand(id, userId);
            await _commandBus.DispatchAsync(command, token);
        }



        [HttpPost("verify")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
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
            var country = User.GetSbCountryClaim();
            var regex = new Regex("^[0-9]+$");
            if (userIdentifier[0] == '0' && regex.IsMatch(userIdentifier))
            {
                userIdentifier = $"+972{userIdentifier.Remove(0, 1)}";
            }

            var query = new UserDetailsQuery(userIdentifier, country);

            var res = await _queryBus.QueryAsync(query, token);
            if (res == null)
            {
                return NotFound();
            }

            res.ProfileUrl = _urlBuilder.BuildProfileEndPoint(res.Id);
            return res;
        }

        [HttpGet("questions")]
        public async Task<IEnumerable<UserQuestionsDto>> GetUserQuestionsDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetSbCountryClaim();
            var query = new UserQuestionsQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("answers")]
        public async Task<IEnumerable<UserAnswersDto>> GetUserAnswersDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetSbCountryClaim();
            var query = new UserAnswersQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }



        [HttpGet("sessions")]
        public async Task<IEnumerable<SessionDto>> SessionsAsync(long id, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new SessionsQuery(id, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("purchased")]
        public async Task<IEnumerable<UserPurchasedDocsDto>> GetUserPurchasedDocsDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetSbCountryClaim();
            var query = new UserPurchasedDocsQuery(id, page, country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("sold")]
        public async Task<IEnumerable<UserSoldItemsDto>> GetUserSoldDocsDetails(long id, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new UserSoldDocsQuery(id, page, country);
            var res = (await _queryBus.QueryAsync(query, token)).ToList();
            foreach (var r in res)
            {
                r.Url = _urlBuilder.BuildDocumentEndPoint(r.ItemId);
            }
            return res;
        }

        [HttpGet("documents")]
        public async Task<IEnumerable<UserDocumentsDto>> GetUserInfo(long id, [FromServices] IBlobProvider blobProvider,
             CancellationToken token)
        {
            var country = User.GetSbCountryClaim();
            var query = new UserDocumentsQuery(id,  country);


            var retVal = (await _queryBus.QueryAsync(query, token)).ToList();
            var tasks = new List<Task>();

            foreach (var document in retVal)
            {
                var file = await _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token).FirstOrDefaultAsync(token);

                if (file != null)
                {
                    document.Preview =
                     await blobProvider.GeneratePreviewLinkAsync(file,
                            TimeSpan.FromMinutes(20));

                    document.SiteLink = Url.RouteUrl("DocumentDownload", new { id = document.Id });
                }
                else
                {
                    var t = _queueProvider.InsertBlobReprocessAsync(document.Id);
                    tasks.Add(t);
                }

            }

            await Task.WhenAll(tasks);

            return retVal;
        }

        [HttpGet("usersFlags")]
        public async Task<UsersFlagsResponse> GetFlags(int minFlags, int page, CancellationToken token)
        {
            var country = User.GetCountryClaim();
            var query = new UserFlagsOthersQuery(minFlags, page, country);
            var res = await _queryBus.QueryAsync(query, token);
            return new UsersFlagsResponse { Flags = res.Item1, Rows = res.Item2 };
        }

        [HttpPut("phone")]
        public async Task<IActionResult> UpdatePhoneAsync(
                [FromBody] UpdatePhoneRequest model, CancellationToken token)
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
        public async Task<IActionResult> UpdateNameAsync(
                [FromBody] UpdateNameRequest model, CancellationToken token)
        {
            var command = new UpdateNameCommand(model.UserId, model.FirstName, model.LastName);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmailAsync(
            [FromBody] UpdateEmailRequest model,
            CancellationToken token)
        {
            try
            {

                var command = new UpdateEmailCommand(model.UserId, model.Email);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DuplicateRowException)
            {
                return BadRequest("this email belongs to someone else");
            }
        }

        [HttpDelete("calendar")]
        public async Task<IActionResult> DeleteGoogleToken([FromQuery(Name = "id")] long userId, CancellationToken token)
        {
            var command = new DeleteGoogleTokenCommand(userId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("note")]
        public async Task<IActionResult> AddNoteAsync(CreateNoteRequest model,
            CancellationToken token)
        {
            var command = new CreateNoteCommand(model.UserId, model.Text, User.GetIdClaim());
            await _commandBus.DispatchAsync(command, token);
            return Ok(User.Identity.Name);
        }

        [HttpGet("notes")]
        public async Task<IEnumerable<UserNoteDto>> GetNotesAsync([FromQuery] long id, CancellationToken token)
        {
            var query = new UserNotesQuery(id);
            return await _queryBus.QueryAsync(query, token);
        }


    }
}