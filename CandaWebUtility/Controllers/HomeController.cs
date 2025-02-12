using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    public class HomeController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {

            if (Session[HighJumpUser.ID] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index", "HighJumpUtility");





            //    //todo put back when login permitted

            //    if (!User.Identity.IsAuthenticated)
            //    {
            //        return RedirectToAction("Login", "Account");
            //    }

            //    if (User.IsInRole(ApplicationRoles.Admin))
            //    {
            //        return RedirectToAction("Index", "Admin");
            //    }

            //    if (User.IsInRole(ApplicationRoles.Staff))
            //    {
            //        return RedirectToAction("Index", "Projects");
            //    }

            //    // if somehow they are in the app without a known role assigned kick them out
            //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //    return RedirectToAction("Login", "Account");

        }
    }
}