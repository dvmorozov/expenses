using System;
using System.Web.Mvc;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : Controller
    {
        //  https://www.evernote.com/shard/s132/nl/14501366/c707248c-3cab-47d7-838a-ec2b791e4ea7
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                return View("Error", new HandleErrorInfo(e, "Mobile", "Index"));
            }
        }
    }
}
