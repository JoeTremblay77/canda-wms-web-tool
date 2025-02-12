using CandaWebUtility.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace CandaWebUtility.Controllers
{
    [Authorize(Roles = ApplicationRoles.Admin)]
    public class UsersAdminController : UserActivityController
    {
        private ApplicationRoleManager _roleManager;

        private ApplicationUserManager _userManager;

        private async Task SendEmailConfirmationTokenAsync(string userID)
        {
            using (Data.EFEntities db = new Data.EFEntities())
            {
                var smtp = db.AccountEmailSMTP.FirstOrDefault();

                using (SmtpClient client = new SmtpClient(smtp.Host, smtp.Port))
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);

                    string token = GlobalCode.GetUserToken(userID, db);

                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { token = token, code = code }, protocol: Request.Url.Scheme);

                    // set defaults in case there is no template
                    string subject = ViewText.ConfirmAccount;
                    string body = "Please confirm your account and create a password by  <a href=\"" + callbackUrl + "\">clicking here</a>. This link expires in 30 minutes.";

                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtp.UserName, SecurityManager.Show(smtp.Password));
                    client.Port = smtp.Port;
                    client.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        var user = await UserManager.FindByIdAsync(userID);
                        message.To.Add(user.Email);
                        message.Subject = subject;
                        message.Body = body;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.From = new MailAddress(smtp.FromEmailAddress);
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.IsBodyHtml = true;

                        await client.SendMailAsync(message);
                    }
                }
            }

            //using (Data.EFEntities db = new Data.EFEntities())
            //{
            //    string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);

            //    string token = GlobalCode.GetUserToken(userID, db);

            //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { token = token, code = code }, protocol: Request.Url.Scheme);

            //    // set defaults in case there is no template
            //    string subject = ViewText.ConfirmAccount;
            //    string body = "Please confirm your account and create a password by  <a href=\"" + callbackUrl + "\">clicking here</a>. This link expires in 30 minutes.";

            //    await UserManager.SendEmailAsync(userID, subject, body);
            //}
        }

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }

            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                _userManager = value;
            }
        }

        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel model, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email.ToLower(), Email = model.Email.ToLower() };
                var adminresult = await UserManager.CreateAsync(user, SecurityManager.TempToken());

                //Add User to the selected Roles
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }

                    using (Data.EFEntities db = new Data.EFEntities())
                    {
                        Data.UserInfo u = new Data.UserInfo();
                        u.UserType = UserTypes.User;
                        u.UserId = user.Id;
                        u.FullName = model.FullName.Trim();
                        u.Token = SecurityManager.GetUniqueKey(Defaults.UserTokenLength);
                        db.UserInfo.Add(u);

                        db.SaveChanges();
                    }

                    await SendEmailConfirmationTokenAsync(user.Id);
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();
                }

                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                using (Data.EFEntities db = new Data.EFEntities())
                {
                    var ui = db.UserInfo.Where(b => b.UserId == user.Id).FirstOrDefault();
                    if (ui != null)
                    {
                        db.UserInfo.Remove(ui);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            string ut = UserTypes.User;
            string fn = string.Empty;
            using (Data.EFEntities db = new Data.EFEntities())
            {
                var ui = db.UserInfo.AsNoTracking().Where(b => b.UserId == user.Id).FirstOrDefault();
                if (ui != null)
                {
                    ut = ui.UserType;
                    fn = ui.FullName;
                }
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                FullName = fn,
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = model.Email.ToLower();
                user.Email = model.Email.ToLower();

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                using (Data.EFEntities db = new Data.EFEntities())
                {
                    var ui = db.UserInfo.Where(b => b.UserId == user.Id).FirstOrDefault();
                    if (ui != null)
                    {
                        ui.UserType = UserTypes.User;
                        ui.FullName = model.FullName.Trim();
                    }

                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            var roles = await UserManager.GetRolesAsync(model.Id);

            return View(new EditUserViewModel()
            {
                FullName = model.FullName,
                Id = model.Id,
                Email = model.Email.ToLower(),
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = roles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        public async Task<ActionResult> Index()
        {
            var list = await UserManager.Users.OrderBy(b => b.UserName).ToListAsync();
            using (Data.EFEntities db = new Data.EFEntities())
            {
                List<EditUserViewModel> model = new List<EditUserViewModel>();
                foreach (var item in list)
                {
                    EditUserViewModel m = new EditUserViewModel();
                    m.Email = item.Email;
                    m.EmailConfirmed = item.EmailConfirmed;
                    m.Id = item.Id;

                    string ut = UserTypes.User;

                    var ui = db.UserInfo.AsNoTracking().Where(b => b.UserId == item.Id).FirstOrDefault();
                    if (ui != null)
                    {
                        ut = ui.UserType;
                    }

                    m.UserType = ut;

                    model.Add(m);
                }

                return View(model);
            }
        }

        public async Task<ActionResult> ResendEmailConfirmation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            if (user != null)
            {
                await SendEmailConfirmationTokenAsync(user.Id);
                return View("ResendEmailConfirmation", (object)user.Email.ToLower());
            }

            return View("Index");
        }
    }
}