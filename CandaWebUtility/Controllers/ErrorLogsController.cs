using CandaWebUtility.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class ErrorLogsController : UserActivityController
    {

        private EFEntities db = new EFEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var model = db.ErrorLog.AsNoTracking().Where(b => b.ErrorLogId == id.Value).FirstOrDefault();
            if (model != null)
            {
                model.DateCreated = GlobalCode.ConvertToApplicationTime(db, model.DateCreated);
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Index()
        {
            string tz = GlobalCode.ApplicationTimeZoneId(db);
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tz);

            var list = db.ErrorLog.AsNoTracking().OrderBy(b => b.DateCreated).ToList();
            foreach (var item in list)
            {
                item.DateCreated = TimeZoneInfo.ConvertTimeFromUtc(item.DateCreated, tzi);
            }

            ViewBag.TimeZone = tz;

            return View(list);
        }
    }
}