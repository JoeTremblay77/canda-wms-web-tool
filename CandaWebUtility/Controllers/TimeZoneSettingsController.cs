
using CandaWebUtility.Data;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class TimeZoneSettingsController : UserActivityController
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
        public ActionResult Create()
        {
            TimeZoneSetting model = new TimeZoneSetting();
            model.TimeZones = GlobalCode.GetTimeZoneList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TimeZoneSetting model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.SettingId = Guid.NewGuid();

                    db.TimeZoneSetting.Add(model);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level1));
                }
            }
            model.TimeZones = GlobalCode.GetTimeZoneList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            TimeZoneSetting timeZoneSetting = db.TimeZoneSetting.Find(id);

            if (timeZoneSetting == null)
            {
                ViewBag.ErrorMsg = ViewText.RecordNotFound;
                return RedirectToAction("Index");
            }
            return View(timeZoneSetting);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                TimeZoneSetting timeZoneSetting = db.TimeZoneSetting.Find(id);

                db.TimeZoneSetting.Remove(timeZoneSetting);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
            }
        }

        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var model = db.TimeZoneSetting.AsNoTracking().Where(m => m.SettingId == id).FirstOrDefault();

                if (model == null)
                {
                    ViewBag.ErrorMsg = ViewText.RecordNotFound;
                    return RedirectToAction("Index");
                }

                model.TimeZones = GlobalCode.GetTimeZoneList();

                return View(model);
            }
            catch (System.Data.DataException ex)
            {
                return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
            }
            catch (Exception ex)
            {
                return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TimeZoneSetting model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TimeZoneSetting edit = db.TimeZoneSetting.Where(m => m.SettingId == model.SettingId).FirstOrDefault();

                    if (edit == null)
                    {
                        ModelState.AddModelError(string.Empty, ViewText.RecordDeleted);
                        return View(model);
                    }

                    edit.TimeZoneId = model.TimeZoneId;

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    LogManager.LogUIException(User.Identity.Name, ex, ErrorSeverity.Level1);
                    ModelState.AddModelError(string.Empty, ViewText.TryAgainToSave);
                }
                catch (RetryLimitExceededException ex)
                {
                    LogManager.LogUIException(User.Identity.Name, ex, ErrorSeverity.Level1);
                    ModelState.AddModelError(string.Empty, ViewText.TryAgainToSave);
                }
                catch (System.Data.Entity.Core.EntityException ex)
                {
                    LogManager.LogUIException(User.Identity.Name, ex, ErrorSeverity.Level1);
                    ModelState.AddModelError(string.Empty, ViewText.TryAgainToSave);
                }
                catch (System.Data.DataException ex)
                {
                    return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
                }
                catch (Exception ex)
                {
                    return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
                }
            }
            model.TimeZones = GlobalCode.GetTimeZoneList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(db.TimeZoneSetting.AsNoTracking().ToList());
        }
    }
}