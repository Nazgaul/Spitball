﻿using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public class AdminConversationController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AdminConversationController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        [HttpGet]
        public async Task<IEnumerable<ConversationDto>> ConversationAsync([FromQuery] ConversationDetailsRequest request
            , CancellationToken token)
        {
            ChatRoomStatus? p = null;
            if (request.Status.HasValue)
            {
                p = Enumeration.FromValue<ChatRoomStatus>(request.Status.Value);
            }

            var query = new ConversationsQuery(request.Id.GetValueOrDefault(), request.Page, User.GetCountryClaim(),
                p, request.AssignTo, request.AutoStatus);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("{identifier}/details")]
        public async Task<IEnumerable<ConversationDetailsDto>> ConversationDetailAsync(
           [FromRoute] string identifier,
           [FromServices] IUrlBuilder urlBuilder,
            CancellationToken token)
        {

            var query = new ConversationDetailsQuery(identifier, User.GetCountryClaim());
            var res = await _queryBus.QueryAsync(query, token);
            return res.Select(item =>
             {
                 item.Image = urlBuilder.BuildUserImageEndpoint(item.UserId, item.Image);
                 return item;
             });
        }


        [HttpGet("{identifier}")]
        public async Task<IEnumerable<ChatMessageDto>> Get(string identifier,
            [FromServices] IUrlBuilder urlBuilder,
            CancellationToken token)
        {
            var result = await _queryBus.QueryAsync(new ChatConversationByIdQuery(identifier, 0, User.GetCountryClaim()), token);
            return result.Select(item =>
            {
                item.Image = urlBuilder.BuildUserImageEndpoint(item.UserId, item.Image);
                return item;
            });
        }

        [HttpPost("{identifier}/status")]
        public async Task<IActionResult> ChangeStatus(
            [FromRoute] string identifier,
            ChangeConversationStatusRequest model,

            CancellationToken token)

        {
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }

            var p = Enumeration.FromValue<ChatRoomStatus>(model.Status);
            if (p == null)
            {
                return BadRequest();
            }

            var command = new ChangeConversationStatusCommand(identifier, p);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("start")]
        public async Task<IActionResult> StartConversation(StartConversationRequest model,
            CancellationToken token)
        {
            var identifier = ChatRoom.BuildChatRoomIdentifier(model.UserId, model.TutorId);
            var command = new SendChatTextMessageCommand(model.Message, model.UserId, identifier, model.TutorId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();

        }

        [HttpPost("{identifier}/assignTo")]
        public async Task<IActionResult> ChangeAssign(
            [FromRoute] string identifier,
            ChangeConversationAssignRequest model,
            [FromServices] ICommandBus commandBus,
            CancellationToken token)

        {
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }
            var command = new ChangeConversationAssignCommand(identifier, model.AssignTo);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("params")]
        public async Task<object> GetParams(CancellationToken token)
        {
            return new
            {
                Status = Enumeration.GetAll<ChatRoomStatus>().GroupBy(x => x.Group).ToDictionary(x => x.Key, y => y),// Enum.GetNames(typeof(ChatRoomStatus)).Select(s=> s.ToCamelCase()),
                AssignTo = await _queryBus.QueryAsync(new AssignToQuery(), token),
                WaitingFor = Enum.GetNames(typeof(WaitingFor)).Select(s => s.ToCamelCase())
            };
        }
    }
}
