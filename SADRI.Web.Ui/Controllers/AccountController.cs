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