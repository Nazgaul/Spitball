using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public sealed class ValidateEmailAttribute : TypeFilterAttribute
    {

        public ValidateEmailAttribute() : base(typeof(ValidateEmailImpl))
        {
        }

        private class ValidateEmailImpl : ActionFilterAttribute
        {
            private readonly IMailProvider _mailProvider;

            public ValidateEmailImpl(IMailProvider mailProvider)
            {
                _mailProvider = mailProvider;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                foreach (var value in context.ActionArguments.Values)
                {
                    var t = ScanObject(value);
                    if (t != null)
                    {
                        var result = await _mailProvider.ValidateEmailAsync(t.ToString(), context.HttpContext.RequestAborted);
                        if (!result)
                        {
                            context.ModelState.AddModelError(string.Empty, "invalid email");
                            context.Result = new BadRequestObjectResult(context.ModelState);
                        }

                        break;
                    }
                   
                }
                await base.OnActionExecutionAsync(context, next);
            }

            private object ScanObject(object obj)
            {

                switch (obj)
                {
                    case CancellationToken _:
                        return null;
                }

                foreach (var property in obj.GetType().GetProperties())
                {
                    object propValue = property.GetValue(obj, null);
                    if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                    {
                        if (property.GetCustomAttribute(typeof(EmailAddressAttribute)) != null)
                        {
                            return propValue;
                        }
                    }

                    else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        return null;
                        //Console.WriteLine("{0}{1}:", indentString, property.Name);
                        //IEnumerable enumerable = (IEnumerable)propValue;
                        //foreach (object child in enumerable)
                        //{

                        //}

                    }
                    else
                    {
                        return ScanObject(propValue);
                    }
                }

                return null;
            }
        }


    }
}