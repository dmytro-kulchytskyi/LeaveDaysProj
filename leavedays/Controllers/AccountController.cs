﻿using leavedays.Models;
using leavedays.Models.Repository;
using leavedays.Models.Repository.Interfaces;
using leavedays.Models.ViewModels.Account;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{

    public class AccountController : Controller
    {
        private readonly string[] CreateUserAllowedRoles = new string[] { "worker", "manager" };


        private readonly CompanyService companyService;
        private readonly UserManager<AppUser, int> userManager;
        private readonly SignInManager<AppUser, int> signInManager;
        private readonly ICompanyRepository companyRepository;
        private readonly IUserRepository userRepository;
        private readonly ILicenseRepository licenseRepository;


        public AccountController(
            UserManager<AppUser, int> userManager,
            SignInManager<AppUser, int> signInManager,
            CompanyService companyService,
            ICompanyRepository companyRepository,
           IUserRepository userRepository,
           ILicenseRepository licenseRepository)
        {
            this.userRepository = userRepository;
            this.companyRepository = companyRepository;
            this.companyService = companyService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.licenseRepository = licenseRepository;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [Authorize]
        public string info(string role = "")
        {
            return User.Identity.IsAuthenticated + " ID: " + User.Identity.GetUserId<int>() + "<br />FAdmin:" + User.IsInRole("FinanceAdmin") + "<br />FAdmin2" + userManager.IsInRole(User.Identity.GetUserId<int>(), "FinanceAdmin");
            var res = userManager.AddToRole(User.Identity.GetUserId<int>(), role);
            //return Content(res.Succeeded.ToString());
        }


        public async Task<ActionResult> CreateCustomer()
        {

            var user = new AppUser() { UserName = "vadosik", Password = "dimas123" };
            user.Roles = "Customer,FinanceAdmin";
            var result = await userManager.CreateAsync(user, "dimas123");
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return Content(result.Succeeded.ToString());
        }

        public string CreateLicense(int n = 1)
        {
            var license = new License();
            license.Name = "l" + n;
            license.Modules = new HashSet<Module>()
            {
                new Module() {Id = 1, Name = "1" },
                 new Module() {Id = 2, Name = "2" }
            };
          
            return "ok " + licenseRepository.Save(license);
        }


        [Authorize]
        public ActionResult AddTo(string role)
        {
            var id = User.Identity.GetUserId<int>();
            userManager.AddToRole(id, role);
            return Content(userManager.IsInRole(id, role).ToString() + "<br /><br />" + string.Join("<br />", userManager.GetRoles(id)));
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
                        var companyId = userRepository.GetByUserName(model.UserName).CompanyId;
                        if(companyId == 0) return RedirectToAction("Index", "Home");
                        var company = companyRepository.GetById(companyId).UrlName;
                        if(string.IsNullOrEmpty(company)) return RedirectToAction("Index", "Home");
                        return RedirectToAction("Company", "Account", new { companyName = company });
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


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            var licenseList = licenseRepository.GetAll();
            var model = new RegisterViewModel();
            model.licenseList = licenseList;
            model.Roles = CreateUserAllowedRoles;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

         
            model.Roles = model.Roles = CreateUserAllowedRoles;
            if (!ModelState.IsValid) return View(model);

            model.CompanyUrl = model.CompanyUrl.ToLower();

            var isUniq = companyRepository.GetByUrlName(model.CompanyUrl) == null;
            if (!isUniq)
            {
                ModelState.AddModelError("", "A company with this URL already exists");
                return View(model);
            }

            var license = licenseRepository.GetByName(model.LicenseName);
             


            List<string> rolesList = new List<string>();
            if (string.IsNullOrEmpty(model.RolesLine))
                rolesList.Add(CreateUserAllowedRoles[0]);

            rolesList = companyService.GetRolesFromLine(model.RolesLine).Select(r => r.ToLower()).Intersect(CreateUserAllowedRoles).ToList();

            if (rolesList.Count == 0 || !rolesList.Contains("customer"))
                rolesList.Add("customer");


            var company = new Company()
            {
                FullName = model.CompanyName,
                UrlName = model.CompanyUrl
            };
            var companyId = companyRepository.Save(company);

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = companyService.GetRolesLine(rolesList),
                CompanyId = companyId,
                Modules = license.Modules
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }
            await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            return RedirectToAction("Index", "Home");
        }

        // [Authorize(Roles="Customer")]
        [HttpGet]
        [Authorize]
        public ActionResult CreateEmployee()
        {
            if (!userManager.IsInRole(User.Identity.GetUserId<int>(), "Customer")) return HttpNotFound();
            var model = new CreateEmployeeViewModel();
            model.Roles = CreateUserAllowedRoles;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            if (!userManager.IsInRole(User.Identity.GetUserId<int>(), "Customer")) return HttpNotFound();
            model.Roles = model.Roles = CreateUserAllowedRoles;
            if (!ModelState.IsValid)
            {
                model.Password = "";
                return View(model);
            }


            List<string> rolesList = new List<string>();
            if (string.IsNullOrEmpty(model.RolesLine))
                rolesList.Add(CreateUserAllowedRoles[0]);

            rolesList = companyService.GetRolesFromLine(model.RolesLine).Select(r => r.ToLower()).Intersect(CreateUserAllowedRoles).ToList();
            if (rolesList.Count == 0)
                rolesList.Add(CreateUserAllowedRoles[0]);

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = companyService.GetRolesLine(rolesList),
                CompanyId = User.Identity.GetUserId<int>()
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateCompany()
        {
            if (!userManager.IsInRole(User.Identity.GetUserId<int>(), "FinanceAdmin")) return HttpNotFound();
            var model = new CreateCompanyViewModel();
            model.Roles = CreateUserAllowedRoles;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateCompany(CreateCompanyViewModel model)
        {

            if (!userManager.IsInRole(User.Identity.GetUserId<int>(), "FinanceAdmin")) return HttpNotFound();
            model.Roles = model.Roles = CreateUserAllowedRoles;
            if (!ModelState.IsValid) return View(model);

            model.CompanyUrl = model.CompanyUrl.ToLower();
           
            var isUniq = companyRepository.GetByUrlName(model.CompanyUrl) == null;
            if (!isUniq)
            {
                ModelState.AddModelError("", "A company with this URL already exists");
                return View(model);
            }

            List<string> rolesList = new List<string>();
            if (string.IsNullOrEmpty(model.RolesLine))
                rolesList.Add(CreateUserAllowedRoles[0]);

            rolesList = companyService.GetRolesFromLine(model.RolesLine).Select(r => r.ToLower()).Intersect(CreateUserAllowedRoles).ToList();

            if (rolesList.Count == 0 || !rolesList.Contains("customer"))
                rolesList.Add("customer");


            var company = new Company()
            {
                FullName = model.CompanyName,
                UrlName = model.CompanyUrl
            };
            var companyId = companyRepository.Save(company);

            var user = new AppUser()
            {
                UserName = model.UserName,
                Roles = companyService.GetRolesLine(rolesList),
                CompanyId = companyId
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error while creating new customer");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Company(string companyName = "")
        {
            if (string.IsNullOrEmpty(companyName)) return RedirectToAction("Index", "Home");
            var company = companyRepository.GetByUrlName(companyName);
            if (company == null) return RedirectToAction("Index", "Home");
            ViewBag.CompanyName = company.FullName;
            return View(userRepository.GetByCompanyId(company.Id));
        }

        [Authorize]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}