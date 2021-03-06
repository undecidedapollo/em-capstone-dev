﻿using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmployeeEvaluationSystem.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Collections.Generic;
using System;
using EmployeeEvaluationSystem.MVC.Models.Authentication;
using System.Web.Routing;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.EF6;
using EmployeeEvaluationSystem.Entity.SharedObjects.Repository.Core;
using Microsoft.Owin.Security;

namespace EmployeeEvaluationSystem.MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private HttpRequestBase passedInRequest;
        private IUnitOfWorkCreator creator;

        public IUnitOfWorkCreator Creator
        {
            get { return creator ?? HttpContext.GetOwinContext().Get<IUnitOfWorkCreator>(); }
            private set { creator = value; }
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IUnitOfWorkCreator creator, HttpRequestBase request = null)
        {
            this.passedInRequest = request;
            UserManager = userManager;
            SignInManager = signInManager;
            this.creator = creator;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
         
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            
            switch (result)
            {
                case SignInStatus.Success:

                    var Grant = SignInManager.AuthenticationManager.AuthenticationResponseGrant;
                    string UserId = Grant.Identity.GetUserId();

                    return RedirectToLocal(UserId, returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, model.RememberMe});
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await SignInManager.HasBeenVerifiedAsync())
                return View("Error");
            return View(new VerifyCodeViewModel {Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe});
        }
       
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
         
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var status = await RegisterUser(model);

                if (status.Item1.Succeeded)
                {
                    var user = status.Item2;

                    await UserManager.AddToRoleAsync(user.Id, "Employee");

                    return View("Info");
                }


                AddErrors(status.Item1);
            }
            return View(model);
        }

        public async Task<Tuple<IdentityResult, ApplicationUser>> RegisterUser(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                EmployeeID = model.EmployeeID,
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                MailingAddress = model.MailingAddress
            };
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var theUser = await UserManager.FindByEmailAsync(user.Email);

                return new Tuple<IdentityResult, ApplicationUser>(result, theUser);
            }

            return new Tuple<IdentityResult, ApplicationUser>(result, null);
        }

        public async Task<MultiRegisterResult> RegisterMultipleUsers(IEnumerable<RegisterViewModel> theItems)
        {
           
            Action<IEnumerable<Tuple<IdentityResult, ApplicationUser>>> tryDeleteUsersFAILSAFE = (users) =>
            {
                try
                {
                    foreach(var user in users)
                    {
                        try
                        {
                            UserManager.Delete(user.Item2);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                catch (Exception)
                {

                }
            };

            var successfulRegistrations = new List<Tuple<IdentityResult, ApplicationUser>>();


            foreach (var user in theItems)
            {
                var result = await this.RegisterUser(user);

                if(result.Item1.Succeeded && result.Item2 != null)
                {
                    successfulRegistrations.Add(result);
                    var daUser = result.Item2;

                    await UserManager.AddToRoleAsync(daUser.Id, "Employee");
                }
                else
                {
                    tryDeleteUsersFAILSAFE(successfulRegistrations);
                    return new MultiRegisterResult { Successful = false, FailedUser = user };
                }
            }
           
            return new MultiRegisterResult { Successful = true };
        }



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {

            if (ModelState.IsValid)
            {
                if(model.password != model.confirmPassword)
                {
                    ModelState.AddModelError("", "Your passwords do not match.");
                    return View(model);
                }

                var pwresult = await UserManager.PasswordValidator.ValidateAsync(model.password);

                if (!pwresult.Succeeded)
                {
                    AddErrors(pwresult);
                    return View(model);
                }

                var confirmed = await UserManager.ConfirmEmailAsync(model.userId, model.emailCode);

                if (!confirmed.Succeeded)
                {
                    AddErrors(confirmed);
                    return View(model);
                }

                var addPW = await UserManager.AddPasswordAsync(model.userId, model.password);

                if (!addPW.Succeeded)
                {
                    //TODO possibly need to unconfirm email if the password step fails.
                    AddErrors(addPW);
                    return View(model);
                }

                return View("SetupPasswordConfirmation");
            }

            return View("Error");
        }

        [AllowAnonymous]
        [HttpGet]
        public  ActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return View("Error");


            return View(new ConfirmEmailViewModel { userId = userId, emailCode = code });
        }

        //
        // GET: /Account/SetupPasswordConfirmation
        [AllowAnonymous]
        public ActionResult SetupPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
         
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !await UserManager.IsEmailConfirmedAsync(user.Id))
                    return View("ForgotPasswordConfirmation");


                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var scheme = Request?.Url?.Scheme ?? passedInRequest.Url.Scheme;
                var hostname = Request?.Url?.Host ?? passedInRequest.Url.Host;

                if ((Request?.Url?.Port ?? passedInRequest.Url.Port) != 80 || (Request?.Url?.Port ?? passedInRequest.Url.Port) != 43)
                {
                    hostname += ":" + (Request?.Url?.Port ?? passedInRequest.Url.Port);
                }

                var theUrl = $"{scheme}://{hostname}/Account/ResetPassword?userId={HttpUtility.UrlEncode(user.Id)}&code={HttpUtility.UrlEncode(code)}";
                
                await UserManager.SendEmailAsync(user.Id, "Reset Password",
                    "Please reset your password by clicking <a href=\"" + theUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
           
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
         
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
         
        public ActionResult ExternalLogin(string provider, string returnUrl){
            
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl}));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
                return View("Error");
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions =
                userFactors.Select(purpose => new SelectListItem {Text = purpose, Value = purpose}).ToList();
            return
                View(new SendCodeViewModel {Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe});
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
         
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
                return View("Error");
            return RedirectToAction("VerifyCode",
                new {Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe});
        }
       
        // POST: /Account/LogOff
        [HttpPost]
         
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public async Task<ActionResult> SendEmailConfirmationTokenAsync(List<string> userIDs, string subject)
        {
            if(userIDs == null)
            {
                userIDs = TempData["usersToRegister"] as List<string>;
            }

            foreach (var userID in userIDs)
            {

                string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);


                var scheme = Request?.Url?.Scheme ?? passedInRequest.Url.Scheme;
                var hostname = Request?.Url?.Host ?? passedInRequest.Url.Host;

                if ((Request?.Url?.Port ?? passedInRequest.Url.Port) != 80 || (Request?.Url?.Port ?? passedInRequest.Url.Port) != 43)
                {
                    hostname += ":" + (Request?.Url?.Port ?? passedInRequest.Url.Port);
                }

                var theUrl = $"{scheme}://{hostname}/Account/ConfirmEmail?userId={HttpUtility.UrlEncode(userID)}&code={HttpUtility.UrlEncode(code)}";
               
                await UserManager.SendEmailAsync(userID, subject,
                    "Please confirm your account by clicking <a href=\"" + theUrl + "\">here</a>");
            }

            return View("EmailSent");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }

        [Authorize]
        public ActionResult RedirectToAppropriatePage(string userId, string returnUrl)
        {
            var user = userId ?? User.Identity.GetUserId();

            return RedirectToLocal(userId, returnUrl);
        }


        public ActionResult RedirectToLocal(string userId, string returnUrl)
        {

            userId = userId ?? User?.Identity?.GetUserId();


            using (var unitOfWork = this.Creator.Create())
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                if (unitOfWork.Users.isUserAdmin(userId))
                {
                    return RedirectToAction("Index", "Cohorts");
                }
                else
                {
                    return RedirectToAction("Index", "UserHub");
                }
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
                if (UserId != null)
                    properties.Dictionary[XsrfKey] = UserId;
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}