
using System.Web.Mvc;


namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class CommentController : BaseController
    {

      


        ///// <summary>
        ///// Render the comment templates
        ///// </summary>
        ///// <returns></returns>
        //[DonutOutputCache(Duration = int.MaxValue, VaryByParam = "none", VaryByCustom = "Lang")] need to put mobile in here
        [ChildActionOnly]
        public ActionResult Comments()
        {
            return PartialView("_Comments");
        }

        /// <summary>
        /// Used in mobile
        /// </summary>
        /// <returns></returns>
        //[DonutOutputCache(Duration = int.MaxValue, VaryByParam = "none", VaryByCustom = "Lang")]
        [ChildActionOnly]
        public ActionResult Wall()
        {
            return PartialView();
        }

       

    }
}
