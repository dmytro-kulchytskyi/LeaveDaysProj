using leavedays.Models;
using leavedays.Models.EditModel;
using leavedays.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace leavedays.Controllers
{
    public class ModuleController : Controller
    {
        readonly RequestService requestService;
        private readonly UserManager<AppUser, int> userManager;

        public ModuleController(RequestService requestService, UserManager<AppUser, int> userManager)
        {
            this.requestService = requestService;
            this.userManager = userManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            EditRequest request = new EditRequest()
            {
                Status = "New",
                UserId = currentUser.Id,
                CompanyId = currentUser.CompanyId,
            };
            return View(request);
        }

        [HttpPost]
        public ActionResult Create(EditRequest request)
        {
            if (!ModelState.IsValid) RedirectToAction("Index", "Home");
            requestService.Save(request);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmNew()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            if (await userManager.IsInRoleAsync(currentUser.Id, "Customer") || await userManager.IsInRoleAsync(currentUser.Id, "FinaceAdmin"))
            {
                return View("RequestPanel", requestService.GetInProgressRequest(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ConfirmNew(int Id, string acceptBtn, string returnUrl = "")
        { 
            if (acceptBtn == "Accept")
            {
                requestService.Accept(Id);
                if (string.IsNullOrEmpty(returnUrl)) return View("Index", "Home");
                return Redirect(returnUrl);
            }
            requestService.Reject(Id);
            if (string.IsNullOrEmpty(returnUrl)) return View("Index", "Home");
            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<ActionResult> ShowConfirmed()
        {
            var currentUser =await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            if (await userManager.IsInRoleAsync(currentUser.Id, "Customer") || await userManager.IsInRoleAsync(currentUser.Id, "FinaceAdmin"))
            {
                return View("ConfirmedRequest", requestService.GetConfirmedRequest(currentUser.CompanyId).OrderBy(model => model.IsAccepted));
            }
            return View("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> ShowSended()
        {
            var currentUser = await userManager.FindByIdAsync(User.Identity.GetUserId<int>());
            if (currentUser == null) return RedirectToAction("Index", "Home");
            return View("UsersRequest", requestService.GetSendedByUserId(currentUser.Id));
        }

        public ActionResult eOwerview()
        {
            return View();
        }
        public ActionResult cOverview()
        {
            return View();
        }
        public ActionResult Pending()
        {
            return View();
        }
    }
}