using CandaWebUtility.Data;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class AccountEmailSMTPController : UserActivityController
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
        public ActionResult Create(AccountEmailSMTP model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.EmailSettingId = Guid.NewGuid();
                    model.DateModified = DateTime.UtcNow;
                    model.UserModified = User.Identity.Name;

                    model.SaveChanges(User.Identity.Name, model);

                    db.AccountEmailSMTP.Add(model);

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

            AccountEmailSMTP model = db.AccountEmailSMTP.Find(id);

            if (model == null)
            {
                ViewBag.ErrorMsg = ViewText.RecordNotFound;
                return RedirectToAction("Index");
            }

            model.Show();

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                AccountEmailSMTP emailSetting = db.AccountEmailSMTP.Find(id);

                db.AccountEmailSMTP.Remove(emailSetting);

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
                var model = db.AccountEmailSMTP.AsNoTracking().Where(m => m.EmailSettingId == id).FirstOrDefault();

                if (model == null)
                {
                    ViewBag.ErrorMsg = ViewText.RecordNotFound;
                    return RedirectToAction("Index");
                }

                model.Show();

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
        public ActionResult Edit(AccountEmailSMTP model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AccountEmailSMTP edit = db.AccountEmailSMTP.Where(m => m.EmailSettingId == model.EmailSettingId).FirstOrDefault();

                    if (edit == null)
                    {
                        ModelState.AddModelError(string.Empty, ViewText.RecordDeleted);
                        return View(model);
                    }

                    edit.SaveChanges(User.Identity.Name, model);

                    db.Entry(edit).OriginalValues["RowVersion"] = model.RowVersion;
                    db.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Edit/" + model.EmailSettingId.ToString());
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (AccountEmailSMTP)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, ViewText.RecordDeleted);
                    }
                    else
                    {
                        var databaseValues = (AccountEmailSMTP)databaseEntry.ToObject();

                        if (databaseValues.FromEmailAddress != clientValues.FromEmailAddress)
                        {
                            ModelState.AddModelError("FromEmailAddress", ViewText.CurrentValue + databaseValues.FromEmailAddress.ToString());
                        }

                        if (databaseValues.Host != clientValues.Host)
                        {
                            ModelState.AddModelError("Host", ViewText.CurrentValue + databaseValues.Host.ToString());
                        }

                        if (databaseValues.Password != clientValues.Password)
                        {
                            ModelState.AddModelError("Password", "Password was overwritten by another user while the record was open. Try to save again");
                        }

                        if (databaseValues.Port != clientValues.Port)
                        {
                            ModelState.AddModelError("Port", ViewText.CurrentValue + databaseValues.Port.ToString());
                        }

                        if (databaseValues.UseSSL != clientValues.UseSSL)
                        {
                            ModelState.AddModelError("UseSSL", ViewText.CurrentValue + databaseValues.UseSSL.ToString());
                        }

                        if (databaseValues.UserName != clientValues.UserName)
                        {
                            ModelState.AddModelError("UserName", ViewText.CurrentValue + databaseValues.UserName.ToString());
                        }

                        if ((databaseValues.UserModified != clientValues.UserModified)
                            || (databaseValues.DateModified != clientValues.DateModified)

                            )
                        {
                            ModelState.AddModelError(string.Empty, ViewText.UnableToSaveDueToUpdateProcess);
                        }

                        ModelState.AddModelError(string.Empty, ViewText.UnableToSave);
                        model.RowVersion = databaseValues.RowVersion;
                    }
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
            return View(db.AccountEmailSMTP.AsNoTracking().ToList());
        }
    }
}