//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Localization;
//using System.Collections;
//using System.ComponentModel.DataAnnotations;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Interfaces;

//namespace Cloudents.Web.Filters
//{
//    public sealed class ValidateEmailAttribute : TypeFilterAttribute
//    {
//        public ValidateEmailAttribute() : base(typeof(ValidateEmailImpl))
//        {
//        }

//        private class ValidateEmailImpl : ActionFilterAttribute
//        {
//            private readonly IMailProvider _mailProvider;
//            private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;

//            public ValidateEmailImpl(IMailProvider mailProvider, IStringLocalizer<DataAnnotationSharedResource> localizer)
//            {
//                _mailProvider = mailProvider;
//                _localizer = localizer;
//            }

//            public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//            {
//                foreach (var value in context.ActionArguments.Values)
//                {
//                    var t = ScanObject(value);
//                    if (t == null) continue;
//                    var result = await _mailProvider.ValidateEmailAsync(t.Value.propertyValue.ToString(), context.HttpContext.RequestAborted);
//                    if (!result)
//                    {
//                        context.ModelState.AddModelError(t.Value.propertyName, _localizer["EmailAddress", t.Value.propertyName]);
//                        context.Result = new BadRequestObjectResult(context.ModelState);
//                    }

//                    break;
//                }
//                await base.OnActionExecutionAsync(context, next);
//            }

//            private static (string propertyName, object propertyValue)? ScanObject(object obj)
//            {
//                switch (obj)
//                {
//                    case CancellationToken _:
//                        return null;
//                }

//                foreach (var property in obj.GetType().GetProperties())
//                {
//                    var propValue = property.GetValue(obj, null);
//                    if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
//                    {
//                        if (property.GetCustomAttribute(typeof(EmailAddressAttribute)) != null)
//                        {
//                            return (property.Name, propValue);
//                        }
//                    }
//                    else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
//                    {
//                        return null;
//                    }
//                    else
//                    {
//                        return ScanObject(propValue);
//                    }
//                }

//                return null;
//            }
//        }
//    }
//}