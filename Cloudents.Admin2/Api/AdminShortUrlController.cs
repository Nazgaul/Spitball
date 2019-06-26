using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminShortUrlController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private IHostingEnvironment HostingEnvironment { get; }
        public AdminShortUrlController(ICommandBus commandBus, IHostingEnvironment hostingEnvironment)
        {
            _commandBus = commandBus;
            HostingEnvironment = hostingEnvironment;
        }


        [HttpPost("url")]

        public async Task<ActionResult<ShortUrlDto>> AddShortUrlAsync([FromBody] AddShortUrlRequest model, CancellationToken token)
        {
            string url;
            if (!HostingEnvironment.IsDevelopment())
            {
                url = $"https://www.spitball.co/{model.Identifier}";
            }
            else
            {
                url = $"https://dev.spitball.co/{model.Identifier}";
            }
            var command = new CreateShortUrlCommand(model.Identifier, model.Destination, model.Expiration);
            await _commandBus.DispatchAsync(command, token);
         
            return new ShortUrlDto(url, model.Destination, model.Expiration);
        }
    }
}
