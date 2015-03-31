using System.Threading.Tasks;
using System;
using System.Web;
using System.Web.Mvc;
//
using System.Transactions;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity;
using SADRI.Web.Ui.ViewModels;
//

namespace SADRI.Web.Ui.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private IAuthenticationManager _authenticationManager;

        public AccountController(ApplicationUserManager userManager, IAuthenticationManager AuthenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = AuthenticationManager;
        }
        
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };

                try
                {
                    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        if (model.External.Equals("No"))//TO-DO Migrar a la VIEW ExternalLoginCallback. Esto no va aca, se debe tener el LoginInfo de la registracion Google 
                        {
   
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded == false)
                        {
                            foreach (var error in result.Errors)
                            {
                                throw new Exception(error);
                            }                            
                        }
                        await _userManager.AddToRoleAsync(user.Id, model.Rol);
                        await SignInAsync(user, isPersistent: false);
                        }
                        else 
                        {
                            var result = await _userManager.CreateAsync(user);

                            if (result.Succeeded == false)
                            {
                                foreach (var error in result.Errors)
                                {
                                    throw new Exception(error);
                                }
                            }

                            Microsoft.AspNet.Identity.UserLoginInfo login = new Microsoft.AspNet.Identity.UserLoginInfo("Google", "https://www.google.com");           
                            result = await _userManager.AddLoginAsync(user.Id, login);
                            if (result.Succeeded)
                                {
                                    await _userManager.AddToRoleAsync(user.Id, model.Rol);                                    
                                    await SignInAsync(user, isPersistent: false);
                                }

                        }
                        transaction.Complete();
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    AddErrors(e.Message);
                }                                
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //Get Login
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (user.LockoutEnabled)
                    {
                        ModelState.AddModelError("", "Invalid username or password Blocked.");
                    }
                    else
                    {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                }
                else
                {
                    try
                    {
                        user = await _userManager.FindByNameAsync(model.UserName);
                        IdentityResult x = await _userManager.AccessFailedAsync(user.Id);
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                    catch (Exception e)
                    {
                        returnUrl = e.Message;
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //GoogleAccount
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                //ViewBag.ReturnUrl = returnUrl;
                //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return RedirectToAction("Error403", "CustomError");
            }
        }
        private class ChallengeResult : HttpUnauthorizedResult
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

            // Used for XSRF protection when adding external logins
            private const string XsrfKey = "SADRI-2015";

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        //FIN GoogleAccount

        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void AddErrors(string error)
        {
            //foreach (var error in result.Errors)
            //{
                ModelState.AddModelError("", error);
            //}
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

    }
}