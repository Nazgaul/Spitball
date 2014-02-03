using System.Web.Mvc;

namespace Zbang.Zbox.Mvc4Students
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}