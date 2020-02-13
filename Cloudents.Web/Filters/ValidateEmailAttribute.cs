using Cloudents.Core.Interfaces;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Filters
{
    public sealed class ValidateEmailAttribute : TypeFilterAttribute
    {
        public ValidateEmailAttribute() : base(typeof(ValidateEmailImpl))
        {
        }

        private sealed class ValidateEmailImpl : ActionFilterAttribute
        {
            private readonly IMailProvider _mailProvider;
            private readonly IStringLocalizer _localizer;

            public ValidateEmailImpl(IMailProvider mailProvider, IStringLocalizerFactory factory)
            {
                _mailProvider = mailProvider;
                var assemblyName = new AssemblyName(typeof(DataAnnotationSharedResource).GetTypeInfo().Assembly.FullName);
                _localizer = factory.Create("DataAnnotationSharedResource", assemblyName.Name);

            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                foreach (var value in context.ActionArguments.Values)
                {
                    var t = ScanObject(value);
                    if (t?.propertyValue == null) continue
                    ;
                    var result = await _mailProvider.ValidateEmailAsync(t.Value.propertyValue.ToString(), context.HttpContext.RequestAborted);
                    if (!result)
                    {
                        var error = _localizer["EmailAddress", t.Value.propertyName];
                        if (error.ResourceNotFound)
                        {
                            error = new LocalizedString("EmailAddress", "This email is not valid");
                        }
                        //this is due to different implementation on client side registration and request tutor
                        context.ModelState.AddModelError("error", error);
                        context.ModelState.AddModelError("Email", error);
                        context.Result = new BadRequestObjectResult(context.ModelState);
                    }

                    break;
                }
                await base.OnActionExecutionAsync(context, next);
            }

            private static (string propertyName, object propertyValue)? ScanObject(object obj)
            {
                switch (obj)
                {
                    case CancellationToken _:
                        return null;
                }

                foreach (var property in obj.GetType().GetProperties())
                {
                    var propValue = property.GetValue(obj, null);
                    if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                    {
                        if (property.GetCustomAttribute(typeof(EmailAddressAttribute)) != null)
                        {
                            return (property.Name, propValue);
                        }
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        return null;
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