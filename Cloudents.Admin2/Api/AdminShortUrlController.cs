using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(/*Roles = Roles.Admin*/)]
    public class AdminShortUrlController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IConfiguration _configuration;
        public AdminShortUrlController(ICommandBus commandBus, IConfiguration configuration)
        {
            _commandBus = commandBus;
            _configuration = configuration;
        }


        [HttpPost("url")]
        public async Task<ActionResult<ShortUrlDto>> AddShortUrlAsync([FromBody] AddShortUrlRequest model, 
             CancellationToken token)
        {
           
            var destinationTest = Uri.TryCreate(model.Destination, UriKind.Absolute, out Uri _);

            if(!destinationTest && !model.Destination.StartsWith("/") && !model.Destination.StartsWith("www"))
            {
                model.Destination = $"/{model.Destination}";
            }

            var command = new CreateShortUrlCommand(model.Identifier, model.Destination, model.Expiration);
            try
            {
                await _commandBus.DispatchAsync(command, token);
            }
            catch (DuplicateRowException)
            {
                return Conflict(); 
            }

            string url = $"{_configuration["Site"]}go/{model.Identifier}";
            return new ShortUrlDto(url, model.Destination, model.Expiration);

        }
    }
}
