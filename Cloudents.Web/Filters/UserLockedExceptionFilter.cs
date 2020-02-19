using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Cloudents.Web.Filters
{
    public class UserLockedExceptionFilter : ExceptionFilterAttribute
    {
        private readonly SignInManager<User> _signInManager;

        public UserLockedExceptionFilter(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            Order = 1;
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