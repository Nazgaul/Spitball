﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Query.Users;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    [Authorize, ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly IQueryBus _queryBus;

        public CouponController(ICommandBus commandBus,  UserManager<User> userManager, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _userManager = userManager;
            _queryBus = queryBus;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostAsync([FromBody]CouponRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var command = new CreateCouponCommand(model.Code, model.CouponType, userId, model.Value, model.Expiration);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                return Ok();
            }
            catch (SqlConstraintViolationException)
            {
                return BadRequest("User need to be a tutor");
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Invalid Value");
            }
            catch (ArgumentException)
            {
                return BadRequest("Value can not be more then 100%");
            }
            return Ok();
        }



        [HttpPost("apply")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ApplyCouponAsync(ApplyCouponRequest model, CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);
                var command = new ApplyCouponCommand(model.Coupon, userId, model.TutorId, model.CourseId);
                await _commandBus.DispatchAsync(command, token);
                return Ok(new
                {
                    Price = command.NewPrice
                });
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid Coupon");
            }
            //catch (DuplicateRowException)
            //{
            //    return BadRequest("This coupon already in use");

            //}
        }

        [HttpGet]
        [Authorize(Policy = "Tutor")]
        public async Task<IEnumerable<CouponDto>> GetUserCouponsAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserCouponsQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }
    }
}
