using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string body = null;
            if (string.Equals(context.HttpContext.Request.Method, "post", StringComparison.OrdinalIgnoreCase))
            {
                if (context.HttpContext.Request.Body.CanSeek)
                {
                    try
                    {
                        context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(context.HttpContext.Request.Body);
                        //{
                         body = await sr.ReadToEndAsync();
                        //}
                    }
                    catch (ObjectDisposedException)
                    {
                        //do nothing
                    }
                }
            }
            var telemetry = new TelemetryClient();
            telemetry.TrackException(context.Exception,new Dictionary<string, string>()
            {
                ["body"] = body
            });
        }
    }

    public class UserLockedExceptionFilter : ExceptionFilterAttribute
    {
        private readonly SignInManager<RegularUser> _signInManager;

        public UserLockedExceptionFilter(SignInManager<RegularUser> signInManager)
        {
            _signInManager = signInManager;
        }

        

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception.GetType() == typeof(UserLockoutException))
            {
                await _signInManager.SignOutAsync();
                context.Result = new UnauthorizedResult();
                context.ExceptionHandled = true;
            }
        }
    }

}
