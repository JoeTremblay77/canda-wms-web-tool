using CandaWebUtility.Data;
using CandaWebUtility.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CandaWebUtility.Controllers
{
    [Authorize]
    public class AccountController : UserActivityController
    {
        private ApplicationSignInManager _signInManager;

        private ApplicationUserManager _userManager;

        private async Task<string> SendEmailConfirmationTokenAsync(string userID)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, ViewText.ConfirmAccount,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        private async Task SendResetPasswordTokenAsync(string userID)
        {
            using (Data.EFEntities db = new Data.EFEntities())
            {
                string code = await UserManager.GeneratePasswordResetTokenAsync(userID);

                string token = GlobalCode.GetUserToken(userID, db);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { token = token, code = code }, protocol: Request.Url.Scheme);

                var oRequest = System.Web.HttpContext.Current.Request;
                string domainURLPrefix = oRequest.Url.GetLeftPart(System.UriPartial.Authority) + System.Web.VirtualPathUtility.ToAbsolute("~/");

                // set defaults in case there is no template
                string subject = "Password Reset";
                string body = "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>. This link expires in 1 hour.";

                //var template = db.EmailTemplate.AsNoTracking().Where(b => b.EmailType == EmailTypes.ResetPassword).FirstOrDefault();

                //if (template != null)
                //{
                //    subject = template.EmailSubject;
                //    body = template.EmailBody;
                //    body = body.Replace(EmailTemplateTags.DomainURLPrefix, domainURLPrefix);
                //    body = body.Replace(EmailTemplateTags.ActionURL, callbackUrl);

                //    Guid imgid = Defaults.MissingAccountImageId;
                //    var imgLink = db.ImageLink.AsNoTracking().Where(b => b.OwnerId == template.MainLogoId).FirstOrDefault();
                //    if (imgLink != null)
                //    {
                //        imgid = imgLink.ImageId;
                //    }

                //    var imgurl = Url.Action("GetImage", "Images", new { id = imgid }, protocol: Request.Url.Scheme);

                //    body = body.Replace(EmailTemplateTags.MainLogoSrc, imgurl);
                //}

                await UserManager.SendEmailAsync(userID, subject, body);
            }
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set { _signInManager = value; }
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string token, string code)
        {
            try
            {
                if (token == null || code == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                string userid = string.Empty;

                using (Data.EFEntities db = new Data.EFEntities())
                {
                    userid = GlobalCode.GetUserIdFromToken(token, db);
                }

                if (string.IsNullOrEmpty(userid))
                {
                    return RedirectToAction("Index", "Home");
                }

                var user = await UserManager.FindByIdAsync(userid);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                bool valid = await UserManager.VerifyUserTokenAsync(user.Id, "Confirmation", code);
                if (!valid)
                {
                    return RedirectToAction("Index", "Home");
                }

                var result = await UserManager.ConfirmEmailAsync(user.Id, code);

                if (!result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ConfirmEmailViewModel model = new ConfirmEmailViewModel
                {
                    UserName = user.UserName
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogManager.LogUIException("Confirm Email Get", ex, ErrorSeverity.Level1);
                return RedirectToAction("Offline");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string ppass1 = model.Password1;
                string ppass2 = model.Password2;

                bool stop = false;

                if (ppass1.Length == 0)
                {
                    stop = true;
                }

                if (ppass2.Length == 0)
                {
                    stop = true;
                }

                if (stop)
                {
                    ModelState.Clear();

                    ModelState.AddModelError("", "An unexpected error occured. Please try again later.");
                    return View(model);
                }

                if (SecurityManager.PasswordInvalid(ppass1))
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Password1", "Password does not meet the requirements");

                    return View(model);
                }

                if (ppass1 != ppass2)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("Password2", "The passwords do not match");

                    return View(model);
                }

                ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var user = await UserManager.FindByEmailAsync(model.UserName);

                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (user.LockoutEndDateUtc != null)
                {
                    if (user.LockoutEndDateUtc.Value < DateTime.UtcNow)
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("", "An account error occured. Please try again later.");
                        return View(model);
                    }
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, code, ppass1);

                if (!result.Succeeded)
                {
                    string ex = string.Empty;
                    foreach (var item in result.Errors)
                    {
                        ex += item + " ";
                    }

                    LogManager.LogUIException("Confirm Email Post Reset", new Exception(ex), ErrorSeverity.Level1);
                    ModelState.Clear();
                    ModelState.AddModelError("", "An unexpected error occured. Please try again later.");
                    return View(model);
                }

                ApplicationSignInManager SignInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

                var result2 = await SignInManager.PasswordSignInAsync(user.UserName, ppass1, false, shouldLockout: true);

                switch (result2)
                {
                    case SignInStatus.Success:
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    case SignInStatus.LockedOut:
                        return RedirectToAction("Lockout");

                    case SignInStatus.Failure:
                    default:
                        ModelState.Clear();
                        ModelState.AddModelError("", "Invalid login attempt.");

                        return View(model);
                }
            }
            catch (Exception ex)
            {
                LogManager.LogUIException("Confirm Email Post", ex, ErrorSeverity.Level1);
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email.ToLower());
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                await SendResetPasswordTokenAsync(user.Id);

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        public ActionResult Index()
        {
            var Db = new ApplicationDbContext();
            var users = Db.Users.OrderBy(b => b.UserName);
            var model = new List<RegisterViewModel>();
            foreach (var user in users)
            {
                var u = new RegisterViewModel
                {
                    Email = user.Email
                };
                model.Add(u);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            using (HJWarehouseEntities db = new HJWarehouseEntities())
            {
                //TODO: Use ODATE TO VALIDATE PASSWORD 

                KoerberLibReturnObject retVal = await KoerberLib.LogOn(model.UserName, model.Password);

                if (retVal.Success)
                {
                    await LogMessagesManager.Log(LogType.Login, model.UserName);
                    Session[HighJumpUser.UserName] = model.UserName;
                    Session[HighJumpUser.ID] = model.UserName;
                    return RedirectToAction("Index", "HighJumpUtility");
                }
                else
                {
                    model.Warning = retVal.Message;
                    return View(model);
                }
                
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string token, string code)
        {
            try
            {
                string userid = string.Empty;

                using (Data.EFEntities db = new Data.EFEntities())
                {
                    userid = GlobalCode.GetUserIdFromToken(token, db);
                }

                if (string.IsNullOrEmpty(userid))
                {
                    return RedirectToAction("Index", "Home");
                }

                var user = UserManager.FindById(userid);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (user.LockoutEndDateUtc != null)
                {
                    if (user.LockoutEndDateUtc.Value > DateTime.UtcNow)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                bool valid = await UserManager.VerifyUserTokenAsync(user.Id, "ResetPassword", code);

                if (!valid)
                {
                    return RedirectToAction("Index", "Home");
                }

                ResetPasswordViewModel model = new ResetPasswordViewModel
                {
                    Code = code,
                    Email = user.Email
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogManager.LogUIException("Reset Password Get", ex, ErrorSeverity.Level1);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool stop = false;
            bool notMatch = false;

            if (model.Code.Length == 0)
            {
                stop = true;
            }

            if (model.Password1.Length == 0)
            {
                stop = true;
            }

            if (model.Password2.Length == 0)
            {
                stop = true;
            }

            if (SecurityManager.PasswordInvalid(model.Password1))
            {
                stop = true;
            }

            if (model.Password1 != model.Password2)
            {
                notMatch = true;
                stop = true;
            }

            if (stop)
            {
                if (notMatch)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                }
                else
                {
                    ModelState.AddModelError("", "Password does not meet requirements.");
                }

                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.Email.Trim().ToLower());
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password1);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #region Helpers

        //////
        ////// POST: /Account/ExternalLogin
        ////[HttpPost]
        ////[AllowAnonymous]
        ////[ValidateAntiForgeryToken]
        ////public ActionResult ExternalLogin(string provider, string returnUrl)
        ////{
        ////    // Request a redirect to the external login provider
        ////    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        ////}

        //////
        ////// GET: /Account/ExternalLoginCallback
        ////[AllowAnonymous]
        ////public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        ////{
        ////    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        ////    if (loginInfo == null)
        ////    {
        ////        return RedirectToAction("Login");
        ////    }

        ////    // Sign in the user with this external login provider if the user already has a login
        ////    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        ////    switch (result)
        ////    {
        ////        case SignInStatus.Success:
        ////        return RedirectToLocal(returnUrl);

        ////        case SignInStatus.LockedOut:
        ////        return View("Lockout");

        ////        case SignInStatus.RequiresVerification:
        ////        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });

        ////        case SignInStatus.Failure:
        ////        default:

        ////        // If the user does not have an account, then prompt the user to create an account
        ////        ViewBag.ReturnUrl = returnUrl;
        ////        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        ////        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        ////    }
        ////}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "UserAccountSelfManage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        //}

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
        //    if (user != null)
        //    {
        //        var token = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);

        //        //    ViewBag.Status = "For debug purposes the current " + provider + " code is: " +token;
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //        return RedirectToLocal(model.ReturnUrl);

        //        case SignInStatus.LockedOut:
        //        return View("Lockout");

        //        case SignInStatus.Failure:
        //        default:
        //        ModelState.AddModelError("", "Invalid code.");
        //        return View(model);
        //    }
        //}

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion Helpers
    }
}