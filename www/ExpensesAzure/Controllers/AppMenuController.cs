using System;
using System.Web.Mvc;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : ErrorHandlingController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                return View("Carousel");
            }
            catch (Exception e)
            {
                return View("AppMenuError", new HandleErrorInfo(e, "Mobile", "Index"));
            }
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            try
            {
                return View("AppMenuError");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
