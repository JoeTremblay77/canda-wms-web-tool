using CandaWebUtility.Data;
using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    [Authorize(Roles = ApplicationRoles.All)]
    public class SettingsController : UserActivityController
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Settings model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.SettingId = Guid.NewGuid();

                    db.Settings.Add(model);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level1));
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Settings settings = db.Settings.Find(id);

            if (settings == null)
            {
                ViewBag.ErrorMsg = ViewText.RecordNotFound;
                return RedirectToAction("Index");
            }
            return View(settings);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                Settings settings = db.Settings.Find(id);

                if (settings == null)
                {
                    ViewBag.ErrorMsg = ViewText.RecordNotFound;
                    return RedirectToAction("Index");
                }

                db.Settings.Remove(settings);

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
                var model = db.Settings.AsNoTracking().Where(m => m.SettingId == id).FirstOrDefault();

                if (model == null)
                {
                    ViewBag.ErrorMsg = ViewText.RecordNotFound;
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (DbUpdateException ex)
            {
                LogManager.LogUIException(User.Identity.Name, ex, ErrorSeverity.Level1);
                ViewBag.ErrorMsg = ViewText.TryAgainToEdit;
            }
            catch (RetryLimitExceededException ex)
            {
                LogManager.LogUIException(User.Identity.Name, ex, ErrorSeverity.Level1);
                ViewBag.ErrorMsg = ViewText.TryAgainToEdit;
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                LogManager.LogUIException(User.Identity.Name, ex, ErrorSeverity.Level1);
                ViewBag.ErrorMsg = ViewText.TryAgainToEdit;
            }
            catch (System.Data.DataException ex)
            {
                return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
            }
            catch (Exception ex)
            {
                return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level2));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Settings model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Settings edit = db.Settings.Where(m => m.SettingId == model.SettingId).FirstOrDefault();

                    if (edit == null)
                    {
                        ModelState.AddModelError(string.Empty, ViewText.RecordDeleted);
                        return View(model);
                    }

                    edit.SettingName = model.SettingName;
                    edit.SettingValue = model.SettingValue;

                    db.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Edit", new { id = model.SettingId });
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

            return View(model);
        }

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var list = db.Settings.AsNoTracking().OrderBy(b => b.SettingName).ToList();

                return View(list);
            }
            catch (Exception ex)
            {
                return View("Error", LogManager.HandleUIException(User.Identity.Name, ex, ErrorSeverity.Level1));
            }
        }
    }
}