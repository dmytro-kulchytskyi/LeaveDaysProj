using leavedays.Models;
using leavedays.Models.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User, int> userManager;
        private readonly SignInManager<User, int> signInManager;
        public AccountController(
            UserManager<User, int> userManager,
            SignInManager<User, int> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }



        public async Task<ActionResult> CreateCustomer(string login="dimas", string pass="dimas")
        {
          
                var user = new User() { UserName = login };
                var result = await userManager.CreateAsync(user, pass);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                }
            return Content(result.Succeeded.ToString());


        }

        [Authorize]
        public ActionResult AddTo(string role)
        {
            userManager.AddToRole(User.Identity.GetUserId<int>(), role);
            return Content(userManager.IsInRole(User.Identity.GetUserId<int>(), role).ToString());
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var user = new User { UserName = model.Email, Password = model.Password };
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/") return RedirectToAction("Index", "Home");
                        return Redirect(returnUrl);
                    }
                //case SignInStatus.LockedOut:
                //    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = "", RememberMe = model.RememberMe });
                //case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
    }
}