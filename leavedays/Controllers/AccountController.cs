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

        private readonly UserManager<AppUser, int> userManager;
        private readonly SignInManager<AppUser, int> signInManager;
        public AccountController(
            UserManager<AppUser, int> userManager,
            SignInManager<AppUser, int> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }



        public async Task<ActionResult> CreateCustomer()
        {
          
                var user = new AppUser() { UserName = "dimas", Password = "dimas123" };  
                var result = await userManager.CreateAsync(user, "dimas123");
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
            return Content(result.Succeeded.ToString());


        }

        
        public ActionResult AddTo(string role)
        {
            userManager.AddToRole(1, role);
            return Content(userManager.IsInRole(1, role).ToString());
        }

        [HttpGet]
        public ActionResult Login()
        {
            //userManager.GetRoles(userId);
            //userManager.IsInRole(userId, roleName);
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
            var user = new AppUser { UserName = model.Email, Password = model.Password };
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