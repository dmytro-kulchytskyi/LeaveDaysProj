using leavedays.Models;
using leavedays.Models.ViewModels.Account;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{

    public class AccountController : Controller
    {
        private readonly CompanyService companyService;

        private readonly UserManager<AppUser, int> userManager;
        private readonly SignInManager<AppUser, int> signInManager;
        public AccountController(
            UserManager<AppUser, int> userManager,
            SignInManager<AppUser, int> signInManager,
            CompanyService companyService)
        {
            this.companyService = companyService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // [Authorize]
        public string info(string role = "")
        {
            return User.Identity.IsAuthenticated + " ID: " + User.Identity.GetUserId<int>();
            var res = userManager.AddToRole(User.Identity.GetUserId<int>(), role);
            //return Content(res.Succeeded.ToString());
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
        [AllowAnonymous]
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
            var user = new AppUser { UserName = model.UserName, Password = model.Password };
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
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


        // [Authorize(Roles="Customer")]
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public ActionResult CreateEmployee()
        {
            var model = new CreateEmployeeViewModel();
            model.Roles = new string[] { "Worker", "Manager" };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeViewModel model, int companyId = 0)
        {
            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }

            var user = new AppUser() { UserName = model.UserName, Roles = ",Customer" + companyService.GetRolesFromLine(model.RolesLine), CompanyId = companyId };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult CreateCompany()
        {
            var model = new CreateCompanyViewModel();
            model.Roles = new string[] { "Worker", "Manager" };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCompany(CreateCompanyViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var company = new Company()
            {
                FullName = model.CompanyName,
                UrlName = string.Join("", model.CompanyName.Split(',', '.', ' ', '_')),

            };
            var companyId = companyService.SaveCompany(company);

            var user = new AppUser() { UserName = model.UserName, Roles = ",customer" + companyService.GetRolesFromLine(model.RolesLine), CompanyId = companyId };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}