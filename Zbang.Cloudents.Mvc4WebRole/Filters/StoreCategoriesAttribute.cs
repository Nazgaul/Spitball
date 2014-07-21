using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class StoreCategoriesAttribute : ActionFilterAttribute
    {
        [Dependency]
        public IZboxCacheReadService ZboxReadService { get; set; }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var categories = ZboxReadService.GetCategories().ToList();
            filterContext.Controller.ViewBag.categories = categories;
            base.OnActionExecuted(filterContext);
        }
    }
}