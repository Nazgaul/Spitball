﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Cloudents.Query.Admin;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public class AdminCouponController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AdminCouponController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

     

        // POST api/<controller>
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody]CouponRequest model, CancellationToken token)
        //{
        //    try
        //    {
        //        var command = new CreateCouponCommand(model.Code,
        //            model.CouponType,
        //            model.TutorId,
        //            model.Value,
        //            model.Expiration,
        //            model.Description
                   
        //        );
        //        await _commandBus.DispatchAsync(command, token);
        //    }
        //    catch (DuplicateRowException)
        //    {
        //        return BadRequest("This coupon already exists");
        //    }
        //    return Ok();
        //}

        [HttpGet]
        public async Task<IEnumerable<CouponDto>> GetAsync(CancellationToken token)
        {
            var query = new CouponQuery();

            return await _queryBus.QueryAsync(query, token);
        }

     
    }
}
