using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class AdminController : UserActivityController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}